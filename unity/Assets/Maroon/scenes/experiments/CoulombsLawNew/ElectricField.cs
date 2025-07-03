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
        public ElectricFieldComputeBuffers computeBuffers; // Cannot initialize here because of computeBuffers

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
            computeBuffers = new ElectricFieldComputeBuffers();
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
            computeBuffers.DisposeBuffers();
        }
    }

    // Contains compute buffers for evaluating the efield inside shaders (See ElectricFieldShaderUtils.cginc)
    public class ElectricFieldComputeBuffers
    {
        public const int MAX_CHARGED_POINTS = 50;
        public const int MAX_CHARGED_RODS = 30;
        public const int MAX_CHARGED_PLANES = 30;

        public ComputeBuffer chargedPointData; // xyz position, w is charge
        public ComputeBuffer chargedRodPositions; // xyz position, w is charge
        public ComputeBuffer chargedRodDirections; // xyz direction, w unused
        public ComputeBuffer chargedPlaneEquations; // xyz normal, w is negative distance from plane to origin
        public ComputeBuffer chargedPlaneChargeDensities; // Note: this is a float buffer, all other are float4

        List<Vector4> cpuChargedPointData = new List<Vector4>();
        List<Vector4> cpuChargedRodPositions = new List<Vector4>();
        List<Vector4> cpuChargedRodDirections = new List<Vector4>();
        List<Vector4> cpuChargedPlaneEquations = new List<Vector4>();
        List<float> cpuChargedPlaneChargeDensities = new List<float>();

        private int lastUpdateFrame = -1;

        public ElectricFieldComputeBuffers()
        {
            chargedPointData = new ComputeBuffer(MAX_CHARGED_POINTS, sizeof(float) * 4);
            chargedRodPositions = new ComputeBuffer(MAX_CHARGED_RODS, sizeof(float) * 4);
            chargedRodDirections = new ComputeBuffer(MAX_CHARGED_RODS, sizeof(float) * 4);
            chargedPlaneEquations = new ComputeBuffer(MAX_CHARGED_PLANES, sizeof(float) * 4);
            chargedPlaneChargeDensities = new ComputeBuffer(MAX_CHARGED_PLANES, sizeof(float));
        }

        public void DisposeBuffers()
        {
            chargedPointData.Dispose();
            chargedRodPositions.Dispose();
            chargedRodDirections.Dispose();
            chargedPlaneEquations.Dispose();
            chargedPlaneChargeDensities.Dispose();
        }

        private void UpdateBuffersForCurrentFrame()
        {
            if (lastUpdateFrame == UnityEngine.Time.frameCount) return;
            lastUpdateFrame = UnityEngine.Time.frameCount;

            cpuChargedPointData.Clear();
            cpuChargedRodPositions.Clear();
            cpuChargedRodDirections.Clear();
            cpuChargedPlaneEquations.Clear();
            cpuChargedPlaneChargeDensities.Clear();

            // Update charged object buffers
            foreach (var pointCharge in ElectricField.Instance.chargedPoints)
            {
                if (cpuChargedPointData.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                cpuChargedPointData.Add(packedInfo);
            }
            chargedPointData.SetData<Vector4>(cpuChargedPointData);

            foreach (var chargedRod in ElectricField.Instance.chargedRods)
            {
                if (cpuChargedRodPositions.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Vector4 packedPos = new Vector4(pos.x, pos.y, pos.z, chargedRod.GetChargeDensity());
                Vector4 packedDir = new Vector4(dir.x, dir.y, dir.z, 0);
                cpuChargedRodPositions.Add(packedPos);
                cpuChargedRodDirections.Add(packedDir);
            }
            chargedRodPositions.SetData<Vector4>(cpuChargedRodPositions);
            chargedRodDirections.SetData<Vector4>(cpuChargedRodDirections);

            // Get charged plane packed data
            foreach (var chargedPlane in ElectricField.Instance.chargedPlanes)
            {
                if (cpuChargedPlaneEquations.Count + 1 >= MAX_CHARGED_PLANES) break;
                var normal = chargedPlane.GetNormal();
                Vector4 equation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, chargedPlane.transform.position));
                cpuChargedPlaneEquations.Add(equation);
                cpuChargedPlaneChargeDensities.Add(chargedPlane.GetChargeDensity());
            }
            chargedPlaneEquations.SetData<Vector4>(cpuChargedPlaneEquations);
            chargedPlaneChargeDensities.SetData<float>(cpuChargedPlaneChargeDensities);
        }

        // Shader should include "ShaderUtils.cginc" for this to work
        public void SetUniformsForComputeShader(ComputeShader computeShader, int kernelIndex)
        {
            UpdateBuffersForCurrentFrame();

            computeShader.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            computeShader.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            computeShader.SetInt("_ChargedPointCount", cpuChargedPointData.Count);
            computeShader.SetInt("_ChargedRodCount", cpuChargedRodPositions.Count);
            computeShader.SetInt("_ChargedPlaneCount", cpuChargedPlaneEquations.Count);
            computeShader.SetBuffer(kernelIndex, "_ChargedPointData", chargedPointData);
            computeShader.SetBuffer(kernelIndex, "_ChargedRodPositions", chargedRodPositions);
            computeShader.SetBuffer(kernelIndex, "_ChargedRodDirections", chargedRodDirections);
            computeShader.SetBuffer(kernelIndex, "_ChargedPlaneEquations", chargedPlaneEquations);
            computeShader.SetBuffer(kernelIndex, "_ChargedPlaneChargeDensities", chargedPlaneChargeDensities);
        }

        public void SetUniformsForMaterial(Material material)
        {
            UpdateBuffersForCurrentFrame();

            material.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            material.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            material.SetInt("_ChargedPointCount", cpuChargedPointData.Count);
            material.SetInt("_ChargedRodCount", cpuChargedRodPositions.Count);
            material.SetInt("_ChargedPlaneCount", cpuChargedPlaneEquations.Count);
            material.SetBuffer("_ChargedPointData", chargedPointData);
            material.SetBuffer("_ChargedRodPositions", chargedRodPositions);
            material.SetBuffer("_ChargedRodDirections", chargedRodDirections);
            material.SetBuffer("_ChargedPlaneEquations", chargedPlaneEquations);
            material.SetBuffer("_ChargedPlaneChargeDensities", chargedPlaneChargeDensities);
        }
    }
}
