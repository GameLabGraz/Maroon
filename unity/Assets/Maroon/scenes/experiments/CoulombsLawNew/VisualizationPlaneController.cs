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

            Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f); // Note: Unity default plane mesh is 10 units in size by default
            var bounds = SimulationBox.Instance.Bounds;
            scale.x *= bounds.max.x - bounds.min.x;
            scale.z *= bounds.max.y - bounds.min.y;

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
            visualizationPlane.transform.rotation = Quaternion.Euler(-90, 0, 0);
            visualizationPlane.transform.position = pos;
        }

        // Updating shader data happens in LateUpdate
        void LateUpdate()
        {
            bool heatmapActive = uiHeatmapToggle.GetValue();
            bool equipotentialLinesActive = uiEquipotentialToggle.GetValue();
            if (!heatmapActive && !equipotentialLinesActive) return;

            // Get packed particle data
            List<Vector4> packedParticles = new List<Vector4>();
            foreach (var pointCharge in ElectricField.Instance.pointCharges)
            {
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                packedParticles.Add(packedInfo);
            }

            // Trim/Resize particle count since we cannot resize the vector array
            //    --> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
            int activeParticleCount = packedParticles.Count;
            const int MAX_COUNT = 100; // Currently hardcoded in shader
            for (var i = packedParticles.Count; i < MAX_COUNT; i++)
            {
                packedParticles.Add(new Vector4(0f, 0f, 0f, 0f));
            }
            if (packedParticles.Count > MAX_COUNT)
            {
                packedParticles.RemoveRange(MAX_COUNT - 1, packedParticles.Count - MAX_COUNT);
            }

            // Get plane equation (See comment in shader about layout)
            var bounds = SimulationBox.Instance.Bounds;
            Vector3 planeCenter = (bounds.max + bounds.min) / 2.0f;
            planeCenter.z = Mathf.Lerp(bounds.min.z, bounds.max.z, uiDepthOffset.GetValue() * .5f + .5f);

            Vector3 planeNormal = visualizationPlane.transform.rotation * Vector3.up; // Unity plane mesh has normal pointing upwards by default
            Vector4 planeEquation = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, -Vector3.Dot(planeNormal, planeCenter));

            // Update shader properties
            var material = visualizationPlane.sharedMaterial;
            material.SetVectorArray(Shader.PropertyToID("_Entries"), packedParticles);
            material.SetInt(Shader.PropertyToID("_EntryCnt"), activeParticleCount);

            material.SetFloat(Shader.PropertyToID("_Transparency"), uiTransparencySlider.GetValue());
            material.SetVector(Shader.PropertyToID("_PlaneEquation"), planeEquation);

            material.SetInteger(Shader.PropertyToID("_DrawHeatmap"), heatmapActive ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_HeatmapMaxVoltage"), uiHeatmapCutoffKiloVoltSlider.GetValue() * 1000.0f);
            material.SetFloat(Shader.PropertyToID("_HeatmapFalloff"), uiHeatmapFalloffSlider.GetValue());

            material.SetInteger(Shader.PropertyToID("_DrawEquipotentialLines"), equipotentialLinesActive ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), uiEquipotentialSpacingSlider.GetValue() * 1000.0f);
            material.SetFloat(Shader.PropertyToID("_LineMaxVoltage"), uiEquipotentialMaximumSlider.GetValue() * 1000.0f);
        }
    }
}
