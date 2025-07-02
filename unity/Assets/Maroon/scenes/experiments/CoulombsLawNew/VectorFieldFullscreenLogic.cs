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
        [SerializeField] private GuiFloatInputHandler uiArrowSizeSlider;

        [SerializeField] private GuiDropdownInputHandler uiColorModeDropdown;
        [SerializeField] private GuiDropdownInputHandler uiScaleModeDropdown;
        [SerializeField] private GuiDropdownInputHandler uiTransparencyModeDropdown;
        [SerializeField] private GuiFloatInputHandler uiFixedTransparencySlider;

        [SerializeField] private GuiFloatInputHandler uiMaxMagnitudeK; // in kilo newton/coulomb
        [SerializeField] private GuiFloatInputHandler uiMagnitudeInterpolationExponent;

        [SerializeField] private GuiFloatInputHandler uiVoltageCenterKV;
        [SerializeField] private GuiFloatInputHandler uiVoltageRangeKV;
        [SerializeField] private GuiFloatInputHandler uiVoltageInterpolationExponent;
        [SerializeField] private GuiBoolInputHandler uiDisplayOutsideOfRangeBool;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering

            vectorFieldValuesBuffer = new ComputeBuffer(
                MAX_RESOLUTION * MAX_RESOLUTION * MAX_RESOLUTION, 4 * 4, ComputeBufferType.Structured, ComputeBufferMode.Dynamic);

            uiTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        private void OnDestroy()
        {
            vectorFieldValuesBuffer.Dispose();
        }

        private void LateUpdate()
        {
            // Calculate vector field values at grid-cell positions, and update compute buffer

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
            vectorFieldMaterial.SetBuffer("_VectorFieldValues", vectorFieldValuesBuffer);
            vectorFieldMaterial.SetMatrix("_InverseView", inverseView);
            
            vectorFieldMaterial.SetVector("_BoxMin", domainOrigin);
            vectorFieldMaterial.SetInt("_FieldResolution", resolution);
            vectorFieldMaterial.SetFloat("_CellSize", cellSize);
            vectorFieldMaterial.SetFloat("_ArrowSize", uiArrowSizeSlider.GetValue());

            vectorFieldMaterial.SetInt("_ColorMode", uiColorModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetInt("_ScaleMode", uiScaleModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetInt("_TransparencyMode", uiTransparencyModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetFloat("_FixedTransparencyValue", uiFixedTransparencySlider.GetValue());

            vectorFieldMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitudeK.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_MagnitudeInterpolationExponent", uiMagnitudeInterpolationExponent.GetValue());

            vectorFieldMaterial.SetFloat("_VoltageCenter", uiVoltageCenterKV.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_VoltageRange",  uiVoltageRangeKV.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_VoltageInterpolationExponent", uiVoltageInterpolationExponent.GetValue());
            vectorFieldMaterial.SetInt("_DisplayOutsideOfRangeBool", uiDisplayOutsideOfRangeBool.GetValue() ? 1 : 0);

            Graphics.Blit(source, destination, vectorFieldMaterial);
        }
    }
}
