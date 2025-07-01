using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VectorFieldFullscreenLogic : MonoBehaviour
    {
        public const int MAX_RESOLUTION = 40;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private ComputeBuffer vectorFieldValuesBuffer;

        [Header("UI-References")]
        [SerializeField] private GuiBoolInputHandler uiEnabledToggle;
        [SerializeField] private GuiIntInputHandler  uiResolutionSlider;

        [SerializeField] private GuiBoolInputHandler uiEnableScalingToggle;
        [SerializeField] private GuiFloatInputHandler uiMaxMagnitude;
        [SerializeField] private GuiFloatInputHandler uiMinMagnitude;
        [SerializeField] private GuiFloatInputHandler uiInterpolationExponent;
        [SerializeField] private GuiFloatInputHandler uiSizeSlider;
        [SerializeField] private GuiFloatInputHandler uiMinSizeSlider;
        [SerializeField] private GuiBoolInputHandler  uiCutoffAboveMaxToggle;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering

            vectorFieldValuesBuffer = new ComputeBuffer(
                MAX_RESOLUTION * MAX_RESOLUTION * MAX_RESOLUTION, 4 * 4, ComputeBufferType.Structured, ComputeBufferMode.Dynamic);
        }

        private void OnDestroy()
        {
            vectorFieldValuesBuffer.Dispose();
        }

        private void LateUpdate()
        {
            var efield = ElectricField.Instance;
            var box = SimulationBox.Instance.Bounds;
            int resolution = uiResolutionSlider.GetValue();

            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            // if (in3DMode) {
            //     maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            // }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();
            var domainOrigin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

            Vector4[] bufferValues = new Vector4[resolution * resolution * resolution];

            // Generate draw calls for each arrow
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int z = 0; z < resolution; z++)
                    {
                        // Get arrow position (At arrow center)
                        Vector3 arrowPos = domainOrigin + cellSize * new Vector3(x, y, z) + cellSize * Vector3.one / 2.0f;

                        // Calculate Electric Field value
                        var fieldValue = efield.GetFieldValue(arrowPos, true); // In [Newton/Coulomb]
                        var potential = efield.GetPotential(arrowPos, true); // In Volt

                        // Note: indexing needs to match with shader
                        bufferValues[x + y * resolution + z * resolution * resolution] = new Vector4(fieldValue.x, fieldValue.y, fieldValue.z, potential);
                    }
                }
            }

            vectorFieldValuesBuffer.SetData(bufferValues, 0, 0, bufferValues.Length);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Early exit if not enabled
            if (!uiEnabledToggle.GetValue())
            {
                Graphics.Blit(source, destination);
                return;
            }
            if (uiEnableScalingToggle.GetValue())
            {
                if (uiMaxMagnitude.GetValue() < uiMinMagnitude.GetValue() || uiMinSizeSlider.GetValue() >= uiSizeSlider.GetValue())
                {
                    Graphics.Blit(source, destination);
                    return;
                }
            }

            var box = SimulationBox.Instance.Bounds;
            int resolution = uiResolutionSlider.GetValue();

            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            // if (in3DMode) {
            //     maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            // }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();
            var domainOrigin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

            // Update shader values
            Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
            vectorFieldMaterial.SetMatrix("_InverseView", inverseView);
            vectorFieldMaterial.SetVector("_BoxMin", domainOrigin);
            vectorFieldMaterial.SetInt("_FieldResolution", resolution);
            vectorFieldMaterial.SetFloat("_CellSize", cellSize);
            vectorFieldMaterial.SetBuffer("_VectorFieldValues", vectorFieldValuesBuffer);

            vectorFieldMaterial.SetInt("_EnableScalingBool", uiEnableScalingToggle.GetValue() ? 1 : 0);
            vectorFieldMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitude.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_MinMagnitude", uiMinMagnitude.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_SizeInterpolationExponent", uiInterpolationExponent.GetValue());
            vectorFieldMaterial.SetFloat("_ArrowSize", uiSizeSlider.GetValue());
            vectorFieldMaterial.SetFloat("_MinArrowSize", uiMinSizeSlider.GetValue());
            vectorFieldMaterial.SetInt("_CutoffAboveMaxBool", uiCutoffAboveMaxToggle.GetValue() ? 1 : 0);

            Graphics.Blit(source, destination, vectorFieldMaterial);
        }
    }
}
