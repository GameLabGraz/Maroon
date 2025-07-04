using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public struct VectorFieldInfos
    {
        public VectorFieldInfos(int resolution, bool vectorField3DMode)
        {
            var box = SimulationBox.Instance.Bounds;
            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            cellSize = maxDimSize / resolution;

            resolutionX = resolution;
            resolutionY = resolution;
            resolutionZ = resolution;
            gridMin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

            // In 2D mode we only show one row of arrows in z
            if (!vectorField3DMode || !CameraController.Instance.In3DMode)
            {
                resolutionZ = 1;
                gridMin.z = box.center.z - cellSize / 2.0f;
            }
        }

        public Vector3 gridMin;
        public float cellSize;
        public int resolutionX;
        public int resolutionY;
        public int resolutionZ;
    };

    public class VectorFieldFullscreenLogic : MonoBehaviour
    {
        public const int MAX_RESOLUTION = 40;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private ComputeShader gridValuesComputeShader;

        private ComputeBuffer gridValuesComputeBuffer;

        [Header("UI-References")]
        [SerializeField] private GUIBoolInputLogic  uiEnabledToggle;
        [SerializeField] private GUIBoolInputLogic  ui3DModeToggle;
        [SerializeField] private GUIIntInputLogic   uiResolutionSlider;
        [SerializeField] private GUIFloatInputLogic uiArrowSizeSlider;

        [SerializeField] private GUIDropdownInputLogic uiColorModeDropdown;
        [SerializeField] private GUIDropdownInputLogic uiScaleModeDropdown;
        [SerializeField] private GUIDropdownInputLogic uiTransparencyModeDropdown;
        [SerializeField] private GUIFloatInputLogic    uiFixedTransparencySlider;
        [SerializeField] private GUIBoolInputLogic     uiDisplayOutsideOfRangeBool;

        [SerializeField] private GUIFloatInputLogic uiMaxMagnitudeK; // in kilo newton/coulomb
        [SerializeField] private GUIFloatInputLogic uiMagnitudeInterpolationExponent;

        [SerializeField] private GUIFloatInputLogic uiVoltageCenterKV;
        [SerializeField] private GUIFloatInputLogic uiVoltageRangeKV;
        [SerializeField] private GUIFloatInputLogic uiVoltageInterpolationExponent;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering

            gridValuesComputeBuffer = new ComputeBuffer(MAX_RESOLUTION * MAX_RESOLUTION * MAX_RESOLUTION, 4 * 4);

            uiTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        private void OnDestroy()
        {
            gridValuesComputeBuffer.Dispose();
        }

        // Calculate vector field values at grid-cell positions, updates compute buffer
        private void LateUpdate()
        {
            VectorFieldInfos gridInfo = new VectorFieldInfos(uiResolutionSlider.GetValue(), ui3DModeToggle.GetValue());

            // Set compute shader uniform values
            int kernelIndex = gridValuesComputeShader.FindKernel("CSMain");
            ElectricField.Instance.computeBuffers.SetUniformsForComputeShader(gridValuesComputeShader, kernelIndex);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_VectorFieldBuffer", gridValuesComputeBuffer);
            gridValuesComputeShader.SetInt("_VectorFieldResolutionX", gridInfo.resolutionX);
            gridValuesComputeShader.SetInt("_VectorFieldResolutionY", gridInfo.resolutionY);
            gridValuesComputeShader.SetInt("_VectorFieldResolutionZ", gridInfo.resolutionZ);
            gridValuesComputeShader.SetVector("_GridMin", new Vector4(gridInfo.gridMin.x, gridInfo.gridMin.y, gridInfo.gridMin.z, 0.0f));
            gridValuesComputeShader.SetFloat("_CellSize", gridInfo.cellSize);

            // Launch compute shader
            uint threadGroupX, threadGroupY, threadGroupZ;
            gridValuesComputeShader.GetKernelThreadGroupSizes(kernelIndex, out threadGroupX, out threadGroupY, out threadGroupZ);
            int requiredGroups = 
                ((gridInfo.resolutionX * gridInfo.resolutionY * gridInfo.resolutionZ) / 
                (int)(threadGroupX * threadGroupY * threadGroupZ)) + 1;
            gridValuesComputeShader.Dispatch(kernelIndex, requiredGroups, 1, 1);


            // CPU-Logic for buffer-values, maybe we want this if ComputeShader is not supported?
            // Vector4[] bufferValues = new Vector4[resolution * resolution * resolution];
            // for (int x = 0; x < resolution; x++)
            // {
            //     for (int y = 0; y < resolution; y++)
            //     {
            //         for (int z = 0; z < resolution; z++)
            //         {
            //             // Get arrow position (At arrow center)
            //             Vector3 arrowPos = domainOrigin + cellSize * new Vector3(x, y, z) + cellSize * Vector3.one / 2.0f;

            //             // Calculate Electric Field value
            //             var fieldValue = efield.GetFieldValue(arrowPos, true); // In [Newton/Coulomb]
            //             var potential = efield.GetPotential(arrowPos, true); // In Volt

            //             // Note: indexing needs to match with shader
            //             bufferValues[x + y * resolution + z * resolution * resolution] = new Vector4(fieldValue.x, fieldValue.y, fieldValue.z, potential);
            //         }
            //     }
            // }
            // vectorFieldValuesBuffer.SetData(bufferValues, 0, 0, bufferValues.Length);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Early exit if not enabled
            if (!uiEnabledToggle.GetValue())
            {
                Graphics.Blit(source, destination);
                return;
            }

            VectorFieldInfos gridInfo = new VectorFieldInfos(uiResolutionSlider.GetValue(), ui3DModeToggle.GetValue());

            // Update shader values
            Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
            vectorFieldMaterial.SetBuffer("_VectorFieldValues", gridValuesComputeBuffer);
            vectorFieldMaterial.SetMatrix("_InverseView", inverseView);

            vectorFieldMaterial.SetVector("_BoxMin", gridInfo.gridMin);
            vectorFieldMaterial.SetInt("_FieldResolutionX", gridInfo.resolutionX);
            vectorFieldMaterial.SetInt("_FieldResolutionY", gridInfo.resolutionY);
            vectorFieldMaterial.SetInt("_FieldResolutionZ", gridInfo.resolutionZ);
            vectorFieldMaterial.SetFloat("_CellSize", gridInfo.cellSize);
            vectorFieldMaterial.SetFloat("_ArrowSize", uiArrowSizeSlider.GetValue());

            vectorFieldMaterial.SetInt("_ColorMode", uiColorModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetInt("_ScaleMode", uiScaleModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetInt("_TransparencyMode", uiTransparencyModeDropdown.GetSelectedIndex());
            vectorFieldMaterial.SetFloat("_FixedTransparencyValue", uiFixedTransparencySlider.GetValue());

            vectorFieldMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitudeK.GetValue());
            vectorFieldMaterial.SetFloat("_MagnitudeInterpolationExponent", uiMagnitudeInterpolationExponent.GetValue());

            vectorFieldMaterial.SetFloat("_VoltageCenter", uiVoltageCenterKV.GetValue());
            vectorFieldMaterial.SetFloat("_VoltageRange", uiVoltageRangeKV.GetValue());
            vectorFieldMaterial.SetFloat("_VoltageInterpolationExponent", uiVoltageInterpolationExponent.GetValue());
            vectorFieldMaterial.SetInt("_DisplayOutsideOfRangeBool", uiDisplayOutsideOfRangeBool.GetValue() ? 1 : 0);

            Graphics.Blit(source, destination, vectorFieldMaterial);
        }
    }
}
