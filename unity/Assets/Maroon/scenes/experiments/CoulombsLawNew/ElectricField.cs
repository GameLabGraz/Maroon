using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ElectricField : MonoBehaviour
    {
        private const float COULOMB_CONSTANT = 1f / (4 * Mathf.PI * Maroon.Physics.PhysicalConstants.e0);

        // Note(MartinR): Electrically charged objects (points, rods, planes) register themselves in these lists
        public List<ChargedPoint> chargedPoints = new List<ChargedPoint>();
        public List<ChargedRod> chargedRods = new List<ChargedRod>();
        public List<ChargedPlane> chargedPlanes = new List<ChargedPlane>();
        public ElectricFieldPackedGPUData electricFieldPackedGPUData; // Cannot initialize here because of computeBuffers

        private void Start()
        {
            // Note(MartinR): My pc keeps churning through frames, and as there are no vsynch options in maroon
            //      i have this here. Remove this before release I guess
            Application.targetFrameRate = 120;
        }

        // Returns the vector-value of the electric field at a given position, Unit: [Newton/Coulomb]
        //      If limitChargeInfluenceDistance is set, then charged objects use a distance threshhold so that
        //      no division by zero/infinitely high values can be produces. This behavior is usually desired
        //      for visualizations and physics calculations (To prevent simulations from exploding due to e.g. overlapping objects)
        public Vector3 GetFieldValue(Vector3 position, bool limitChargeInfluenceDistance, GameObject excludeObject = null)
        {
            Vector3 fieldValue = Vector3.zero;

            // Add point charge influence
            foreach (var chargedPoint in chargedPoints)
            {
                if (chargedPoint.gameObject == excludeObject) continue;

                var toChargeDirection = position - chargedPoint.transform.position;
                float distanceInMeter = toChargeDirection.magnitude;
                toChargeDirection = toChargeDirection.normalized; // Note(MartinR): This creates a zero-vector if the position is exactly the charge pos
                if (limitChargeInfluenceDistance) 
                {
                    distanceInMeter = Mathf.Max(distanceInMeter, ChargedPoint.RADIUS); 
                }

                fieldValue += toChargeDirection * chargedPoint.GetCharge() * COULOMB_CONSTANT / (distanceInMeter * distanceInMeter);
            }

            // Add rod influences
            foreach (var chargedRod in chargedRods)
            {
                if (chargedRod.gameObject == excludeObject) continue;

                Vector3 direction = chargedRod.GetDirection(); // Should be normalized
                Vector3 rodPos = chargedRod.transform.position;

                Vector3 positionProjectedOnRod = rodPos + direction * Vector3.Dot(position - rodPos, direction);
                Vector3 rodToPositionDir = position - positionProjectedOnRod;
                float distanceToRod = rodToPositionDir.magnitude;
                rodToPositionDir = rodToPositionDir.normalized;
                if (limitChargeInfluenceDistance)
                {
                    distanceToRod = Mathf.Max(distanceToRod, ChargedRod.RADIUS);
                }

                fieldValue += chargedRod.GetChargeDensity() * 2 * COULOMB_CONSTANT * rodToPositionDir / distanceToRod;
            }

            // Add plane influences
            foreach (var chargedPlane in chargedPlanes)
            {
                if (chargedPlane.gameObject == excludeObject) continue;

                var normal = chargedPlane.GetNormal();
                var pointOnPlane = chargedPlane.transform.position;
                var signedDistance = Vector3.Dot(normal, position - pointOnPlane);

                fieldValue += chargedPlane.GetChargeDensity() * (2 * Mathf.PI * COULOMB_CONSTANT) * Mathf.Sign(signedDistance) * normal;
            }

            return fieldValue;
        }

        // Returns the electric potential (In Volt) at a given position, parameters are similar to GetFieldValue
        public float GetPotential(Vector3 position, bool limitChargeInfluenceDistance, GameObject excludeObject = null)
        {
            float potential = 0.0f;

            // Add point charge influence
            foreach (var chargedPoint in chargedPoints)
            {
                if (chargedPoint.gameObject == excludeObject) continue;

                float distanceInMeter = (position - chargedPoint.transform.position).magnitude;
                if (limitChargeInfluenceDistance) { 
                    distanceInMeter = Mathf.Max(distanceInMeter, ChargedPoint.RADIUS); 
                }

                potential += chargedPoint.GetCharge() * COULOMB_CONSTANT / distanceInMeter;
            }

            // Add rod influences
            foreach (var chargedRod in chargedRods)
            {
                if (chargedRod.gameObject == excludeObject) continue;

                Vector3 direction = chargedRod.GetDirection(); // Should be normalized
                Vector3 rodPos = chargedRod.transform.position;

                Vector3 positionProjectedOnRod = rodPos + direction * Vector3.Dot(position - rodPos, direction);
                Vector3 rodToPositionDir = position - positionProjectedOnRod;
                float distanceToRod = rodToPositionDir.magnitude;
                rodToPositionDir = rodToPositionDir.normalized;
                if (limitChargeInfluenceDistance)
                {
                    distanceToRod = Mathf.Max(distanceToRod, ChargedRod.RADIUS);
                }

                potential += -chargedRod.GetChargeDensity() * 2 * COULOMB_CONSTANT * Mathf.Log(distanceToRod);
            }

            // Add plane influences
            foreach (var chargedPlane in chargedPlanes)
            {
                if (chargedPlane.gameObject == excludeObject) continue;

                var normal = chargedPlane.GetNormal();
                var pointOnPlane = chargedPlane.transform.position;
                var signedDistance = Vector3.Dot(normal, position - pointOnPlane);

                potential += -chargedPlane.GetChargeDensity() * Mathf.Abs(signedDistance) * (2 * Mathf.PI * COULOMB_CONSTANT);
            }

            return potential;
        }



        // Singleton pattern, see SimulationBox.cs for more comments about the implementation
        private static ElectricField _instance;

        public static ElectricField Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ElectricField>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            electricFieldPackedGPUData = new ElectricFieldPackedGPUData();
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
        }
    }

    // Contains compute buffers for evaluating the efield inside shaders (See ElectricFieldShaderUtils.cginc)
    public class ElectricFieldPackedGPUData
    {
        private const int MAX_CHARGED_POINTS = 50;
        private const int MAX_CHARGED_RODS = 25;
        private const int MAX_CHARGED_PLANES = 25;
        private const int PACKED_TEXTURE_WIDTH = 16;

        private int lastUpdateFrame = -1;

        // Data is linearized into a float4 array, which is then stored in the
        // texture with the following conversion: linear_index = texture_x + texture_y * TEXTURE_WIDTH
        // See UpdateBuffersForCurrentFrame on how the charged objects are linearized
        private Texture2D packedDataTexture;

        public ElectricFieldPackedGPUData()
        {
            packedDataTexture = new Texture2D(PACKED_TEXTURE_WIDTH, PACKED_TEXTURE_WIDTH, TextureFormat.RGBAFloat, false, true);
        }

        private static int intMin(int a, int b)
        {
            return a < b ? a : b;
        }

        private void UpdateBuffersForCurrentFrame()
        {
            if (lastUpdateFrame == UnityEngine.Time.frameCount) return;
            lastUpdateFrame = UnityEngine.Time.frameCount;

            // Update packed data
            var efield = ElectricField.Instance;
            Unity.Collections.NativeArray<Vector4> rawTextureData = packedDataTexture.GetRawTextureData<Vector4>();
            int linearIndex = 0;

            for (int i = 0; i < intMin(efield.chargedPoints.Count, MAX_CHARGED_POINTS); i++)
            {
                var point = efield.chargedPoints[i];
                var pos = point.transform.position;

                rawTextureData[linearIndex] = new Vector4(pos.x, pos.y, pos.z, point.GetCharge());
                linearIndex += 1;
            }
            for (int i = 0; i < intMin(efield.chargedRods.Count, MAX_CHARGED_RODS); i++)
            {
                var rod = efield.chargedRods[i];
                var pos = rod.transform.position;
                var dir = rod.GetDirection();
                float charge = rod.GetChargeDensity();

                rawTextureData[linearIndex] = new Vector4(pos.x, pos.y, pos.z, charge);
                linearIndex += 1;
                rawTextureData[linearIndex] = new Vector4(dir.x, dir.y, dir.z, charge);
                linearIndex += 1;
            }
            for (int i = 0; i < intMin(efield.chargedPlanes.Count, MAX_CHARGED_PLANES); i++)
            {
                var plane = efield.chargedPlanes[i];
                var planeEquation = plane.GetPlaneEquation();
                float charge = plane.GetChargeDensity();

                rawTextureData[linearIndex] = planeEquation;
                linearIndex += 1;
                rawTextureData[linearIndex] = charge * Vector4.one;
                linearIndex += 1;
            }
            // Upload packed data to gpu texture
            packedDataTexture.Apply();
        }

        // Shader should include "ElectricFieldShaderUtils.cginc" for this to work
        public void SetUniformsForComputeShader(ComputeShader computeShader, int kernelIndex)
        {
            UpdateBuffersForCurrentFrame();

            var efield = ElectricField.Instance;
            computeShader.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            computeShader.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            computeShader.SetInt("_ChargedPointCount", intMin(MAX_CHARGED_POINTS, efield.chargedPoints.Count));
            computeShader.SetInt("_ChargedRodCount", intMin(MAX_CHARGED_RODS, efield.chargedRods.Count));
            computeShader.SetInt("_ChargedPlaneCount", intMin(MAX_CHARGED_PLANES, efield.chargedPlanes.Count));
            computeShader.SetInt("_ChargedObjectTextureWidth", PACKED_TEXTURE_WIDTH);
            computeShader.SetTexture(kernelIndex, "_ChargedObjectDataPacked", packedDataTexture);
        }

        public void SetUniformsForMaterial(Material material)
        {
            UpdateBuffersForCurrentFrame();

            var efield = ElectricField.Instance;
            material.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            material.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            material.SetInt("_ChargedPointCount", intMin(MAX_CHARGED_POINTS, efield.chargedPoints.Count));
            material.SetInt("_ChargedRodCount", intMin(MAX_CHARGED_RODS, efield.chargedRods.Count));
            material.SetInt("_ChargedPlaneCount", intMin(MAX_CHARGED_PLANES, efield.chargedPlanes.Count));
            material.SetInt("_ChargedObjectTextureWidth", PACKED_TEXTURE_WIDTH);
            material.SetTexture("_ChargedObjectDataPacked", packedDataTexture);
        }
    }
}
