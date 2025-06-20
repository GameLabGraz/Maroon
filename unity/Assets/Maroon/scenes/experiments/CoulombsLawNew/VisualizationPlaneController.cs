using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VisualizationPlaneController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer visualizationPlane;

        [Header("UI references")]
        [SerializeField] private GuiFloatInputHandler uiDepthOffset;
        [SerializeField] private GuiFloatInputHandler uiTransparencySlider;

        [SerializeField] private GuiBoolInputHandler uiHeatmapToggle;
        [SerializeField] private GuiFloatInputHandler uiHeatmapCutoffKiloVoltSlider;
        [SerializeField] private GuiFloatInputHandler uiHeatmapFalloffSlider;

        [SerializeField] private GuiBoolInputHandler uiEquipotentialToggle;
        [SerializeField] private GuiFloatInputHandler uiEquipotentialMaximumSlider;
        [SerializeField] private GuiFloatInputHandler uiEquipotentialSpacingSlider;

        private void Awake()
        {
            // Register Callbacks
            uiHeatmapToggle.OnValueChanged.AddListener((bool _unused) => { UpdateVisualizationPlane(); });
            uiEquipotentialToggle.OnValueChanged.AddListener((bool _unused) => { UpdateVisualizationPlane(); });
            uiDepthOffset.OnValueChanged.AddListener((float _unused) => { UpdateVisualizationPlane(); });
            CameraController.Instance.OnCameraModeChanged.AddListener(() => { UpdateVisualizationPlane(); });
            SimulationBox.Instance.OnBoundsChanged.AddListener((Bounds _unused) => { UpdateVisualizationPlane(); });
            UpdateVisualizationPlane();
        }

        private void UpdateVisualizationPlane()
        {
            bool heatmapActive = uiHeatmapToggle.GetValue();
            bool equipotentialLinesActive = uiEquipotentialToggle.GetValue();

            bool planeVisible = heatmapActive || equipotentialLinesActive;
            visualizationPlane.enabled = planeVisible;
            if (!planeVisible) return;

            Vector3 scale = Vector3.one; // Note: Unity quad mesh is 1x1
            var bounds = SimulationBox.Instance.Bounds;
            scale.x *= bounds.max.x - bounds.min.x;
            scale.y *= bounds.max.y - bounds.min.y;

            var pos = (bounds.max + bounds.min) / 2.0f;
            if (CameraController.Instance.In3DMode)
            {
                pos.z = Mathf.Lerp(bounds.min.z, bounds.max.z, uiDepthOffset.GetValue() * .5f + .5f);
            }
            else
            {
                pos.z = bounds.max.z + 3.0f;
            }

            visualizationPlane.transform.localScale = scale;
            visualizationPlane.transform.rotation = Quaternion.identity;
            visualizationPlane.transform.position = pos;
        }

        // Updating shader data happens in LateUpdate
        void LateUpdate()
        {
            bool heatmapActive = uiHeatmapToggle.GetValue();
            bool equipotentialLinesActive = uiEquipotentialToggle.GetValue();
            if (!heatmapActive && !equipotentialLinesActive) return;

            // Get packed particle data
            List<Vector4> pointChargeData = new List<Vector4>();
            foreach (var pointCharge in ElectricField.Instance.chargedPoints)
            {
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                pointChargeData.Add(packedInfo);
            }

            // Trim/Resize particle count since we cannot resize the vector array
            //    --> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
            int activePointCharges = pointChargeData.Count;
            const int MAX_POINT_CHARGES = 100; // Currently hardcoded in shader
            for (var i = pointChargeData.Count; i < MAX_POINT_CHARGES; i++)
            {
                pointChargeData.Add(new Vector4(0f, 0f, 0f, 0f));
            }
            if (pointChargeData.Count > MAX_POINT_CHARGES)
            {
                pointChargeData.RemoveRange(MAX_POINT_CHARGES - 1, pointChargeData.Count - MAX_POINT_CHARGES);
            }

            // Get packed charged rod data
            List<Vector4> chargedRodPositions = new List<Vector4>();
            List<Vector4> chargedRodDirections = new List<Vector4>();
            foreach (var chargedRod in ElectricField.Instance.chargedRods)
            {
                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Vector4 packedPos = new Vector4(pos.x, pos.y, pos.z, chargedRod.GetChargeDenstiy());
                Vector4 packedDir = new Vector4(dir.x, dir.y, dir.z, 0);
                chargedRodPositions.Add(packedPos);
                chargedRodDirections.Add(packedDir);
            }

            int activeChargedRodCount = chargedRodPositions.Count;
            const int MAX_CHARGED_RODS = 30; // Currently hardcoded in shader
            for (var i = chargedRodPositions.Count; i < MAX_CHARGED_RODS; i++)
            {
                chargedRodPositions.Add(Vector4.zero);
                chargedRodDirections.Add(Vector4.zero);
            }
            if (chargedRodPositions.Count > MAX_CHARGED_RODS)
            {
                chargedRodPositions.RemoveRange(MAX_CHARGED_RODS - 1, chargedRodPositions.Count - MAX_CHARGED_RODS);
                chargedRodDirections.RemoveRange(MAX_CHARGED_RODS - 1, chargedRodDirections.Count - MAX_CHARGED_RODS);
            }

            // Get charged plane packed data
            List<Vector4> chargedPlaneEquations = new List<Vector4>();
            List<float> chargedPlaneChargeDensities = new List<float>();
            foreach (var chargedPlane in ElectricField.Instance.chargedPlanes)
            {
                var normal = chargedPlane.GetNormal();
                Vector4 planeEquation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, chargedPlane.transform.position));
                chargedPlaneEquations.Add(planeEquation);
                chargedPlaneChargeDensities.Add(chargedPlane.GetChargeDensity());
            }

            int activeChargedPlanes = chargedPlaneEquations.Count;
            const int MAX_CHARGED_PLANES = 30; // Currently hardcoded in shader
            for (var i = chargedPlaneEquations.Count; i < MAX_CHARGED_PLANES; i++)
            {
                chargedPlaneEquations.Add(Vector4.zero);
                chargedPlaneChargeDensities.Add(0.0f);
            }
            if (chargedPlaneEquations.Count > MAX_CHARGED_PLANES)
            {
                chargedPlaneEquations.RemoveRange(MAX_CHARGED_PLANES - 1, chargedPlaneEquations.Count - MAX_CHARGED_PLANES);
                chargedPlaneChargeDensities.RemoveRange(MAX_CHARGED_PLANES - 1, chargedPlaneChargeDensities.Count - MAX_CHARGED_PLANES);
            }

            // Get plane equation (See comment in shader about layout)
            var bounds = SimulationBox.Instance.Bounds;
            Vector3 planeCenter = (bounds.max + bounds.min) / 2.0f;
            planeCenter.z = Mathf.Lerp(bounds.min.z, bounds.max.z, uiDepthOffset.GetValue() * .5f + .5f);

            Vector3 planeNormal = visualizationPlane.transform.rotation * Vector3.back; // Unity quad mesh has normal pointing towards negative z
            Vector4 visualizationPlaneEquation = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, -Vector3.Dot(planeNormal, planeCenter));

            // Update shader properties
            var material = visualizationPlane.sharedMaterial;
            material.SetInt(Shader.PropertyToID("_PointChargeCount"), activePointCharges);
            material.SetVectorArray(Shader.PropertyToID("_PointChargeData"), pointChargeData);
            material.SetInt(Shader.PropertyToID("_ChargedRodCount"), activeChargedRodCount);
            material.SetVectorArray(Shader.PropertyToID("_ChargedRodPositions"), chargedRodPositions);
            material.SetVectorArray(Shader.PropertyToID("_ChargedRodDirections"), chargedRodDirections);
            material.SetInt(Shader.PropertyToID("_ChargedPlaneCount"), activeChargedPlanes);
            material.SetVectorArray(Shader.PropertyToID("_ChargedPlaneEquations"), chargedPlaneEquations);
            material.SetFloatArray(Shader.PropertyToID("_ChargedPlaneChargeDensities"), chargedPlaneChargeDensities);

            material.SetFloat(Shader.PropertyToID("_Transparency"), uiTransparencySlider.GetValue());
            material.SetVector(Shader.PropertyToID("_PlaneEquation"), visualizationPlaneEquation);

            material.SetInteger(Shader.PropertyToID("_DrawHeatmap"), heatmapActive ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_HeatmapMaxVoltage"), uiHeatmapCutoffKiloVoltSlider.GetValue() * 1000.0f);
            material.SetFloat(Shader.PropertyToID("_HeatmapFalloff"), uiHeatmapFalloffSlider.GetValue());

            material.SetInteger(Shader.PropertyToID("_DrawEquipotentialLines"), equipotentialLinesActive ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), uiEquipotentialSpacingSlider.GetValue() * 1000.0f);
            material.SetFloat(Shader.PropertyToID("_LineMaxVoltage"), uiEquipotentialMaximumSlider.GetValue() * 1000.0f);
        }
    }
}
