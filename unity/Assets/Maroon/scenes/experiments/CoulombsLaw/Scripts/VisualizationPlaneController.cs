using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLaw
{
    public class VisualizationPlaneController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer voltageHeatmapPlane;
        [SerializeField] private MeshRenderer equipotentialLinesPlane;
        [SerializeField] private ParticleController particleController;

        [SerializeField] private UIFloatInput heatmapFalloff;
        [SerializeField] private UIFloatInput heatmapMaxKiloVolt;

        [SerializeField] private UIFloatInput spacingKiloVoltage;
        [SerializeField] private UIFloatInput maxEquipotentialKiloVoltage;

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

                voltageHeatmapPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_Falloff"), heatmapFalloff.GetValue());
                voltageHeatmapPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_MaxAbsoluteVoltage"), heatmapMaxKiloVolt.GetValue() * 1000.0f);
            }
            if (equipotentialLinesPlane.gameObject.activeInHierarchy)
            {
                equipotentialLinesPlane.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCnt"), activeParticleCount);
                equipotentialLinesPlane.sharedMaterial.SetVectorArray(Shader.PropertyToID("_Entries"), packedParticles);

                equipotentialLinesPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), spacingKiloVoltage.GetValue() * 1000.0f);
                equipotentialLinesPlane.sharedMaterial.SetFloat(Shader.PropertyToID("_MaxAbsLineVoltage"), maxEquipotentialKiloVoltage.GetValue() * 1000.0f);

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
