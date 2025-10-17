using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    public class VisualizationPlaneController : MonoBehaviour
    {
        [Header("Game object references")]
        [SerializeField] private MeshRenderer voltageHeatmapPlane;
        [SerializeField] private MeshRenderer equipotentialLinesPlane;
        [SerializeField] private ParticleController particleController;
        [SerializeField] private VectorFieldController vectorFieldController;

        [Header("UI references")]
        [SerializeField] private Toggle uiVectorFieldToggle;
        [SerializeField] private Slider uiVectorFieldResolutionSlider;

        [SerializeField] private Toggle uiHeatmapToggle;
        [SerializeField] private UIFloatInput uiHeatmapCutoffKiloVoltSlider;
        [SerializeField] private UIFloatInput uiHeatmapFalloffSlider;

        [SerializeField] private Toggle uiEquipotentialToggle;
        [SerializeField] private UIFloatInput uiEquipotentialMaximumSlider;
        [SerializeField] private UIFloatInput uiEquipotentialSpacingSlider;

        private void Awake()
        {
            // Register UI-Callbacks
            uiVectorFieldToggle.onValueChanged.AddListener((bool enabled) =>
            {
                vectorFieldController.Visible = enabled;
            });
            uiVectorFieldResolutionSlider.onValueChanged.AddListener((float resolution) =>
            {
                vectorFieldController.SetResolutionFromFloat(resolution);
            });

            uiHeatmapToggle.onValueChanged.AddListener((bool enabled) =>
            {
                voltageHeatmapPlane.gameObject.SetActive(enabled);
            });

            uiEquipotentialToggle.onValueChanged.AddListener((bool enabled) =>
            {
                equipotentialLinesPlane.gameObject.SetActive(enabled);
            });
        }

        void LateUpdate()
        {
            // Trim/Resize particle count since we cannot resize the vector array
            //    --> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
            var packedParticles = particleController.GetPackedParticleInfo();
            int activeParticleCount = packedParticles.Count;
            const int MAX_COUNT = 100; // Set in shader
            for (var i = packedParticles.Count; i < MAX_COUNT; i++)
            {
                packedParticles.Add(new Vector4(0f, 0f, 0f, 0f));
            }
            if (packedParticles.Count > MAX_COUNT)
            {
                packedParticles.RemoveRange(MAX_COUNT - 1, packedParticles.Count - MAX_COUNT);
            }

            // Update shader values
            if (voltageHeatmapPlane.gameObject.activeInHierarchy)
            {
                voltageHeatmapPlane.sharedMaterial.SetVectorArray(Shader.PropertyToID("_Entries"), packedParticles);
                voltageHeatmapPlane.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCnt"), activeParticleCount);

                voltageHeatmapPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_Falloff"), uiHeatmapFalloffSlider.GetValue());
                voltageHeatmapPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_MaxAbsoluteVoltage"), uiHeatmapCutoffKiloVoltSlider.GetValue() * 1000.0f);
            }
            if (equipotentialLinesPlane.gameObject.activeInHierarchy)
            {
                equipotentialLinesPlane.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCnt"), activeParticleCount);
                equipotentialLinesPlane.sharedMaterial.SetVectorArray(Shader.PropertyToID("_Entries"), packedParticles);

                equipotentialLinesPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), uiEquipotentialSpacingSlider.GetValue() * 1000.0f);
                equipotentialLinesPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_MaxAbsLineVoltage"), uiEquipotentialMaximumSlider.GetValue() * 1000.0f);

            }
        }

        public void ToggleVoltageHeatmap()
        {
            voltageHeatmapPlane.gameObject.SetActive(!voltageHeatmapPlane.gameObject.activeSelf);
        }
        public void SetVoltageHeatmapActive(bool isActive)
        {
            voltageHeatmapPlane.gameObject.SetActive(isActive);
        }

        public void ToggleEquipotentialLines()
        {
            equipotentialLinesPlane.gameObject.SetActive(!equipotentialLinesPlane.gameObject.activeSelf);
        }
        public void SetEquipotentialLinesActive(bool isActive)
        {
            equipotentialLinesPlane.gameObject.SetActive(isActive);
        }
    }
}
