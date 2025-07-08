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

    public class FullscreenShaderVisualizations : MonoBehaviour
    {
        public const int VECTORFIELD_MAX_RESOLUTION = 40;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private Material isoSurfaceMaterial;
        [SerializeField] private ComputeShader gridValuesComputeShader;
        [SerializeField] private GroundPinLogic groundPin;

        private ComputeBuffer gridValuesComputeBuffer;

        [Header("UI-Vector-Field References")]
        [SerializeField] private GUIBoolInputLogic  uiVectorFieldEnabledToggle;
        [SerializeField] private GUIBoolInputLogic  uiVectorField3DModeToggle;
        [SerializeField] private GUIIntInputLogic   uiVectorFieldResolutionSlider;
        [SerializeField] private GUIFloatInputLogic uiVectorFieldArrowSizeSlider;
        [SerializeField] private GUIDropdownInputLogic uiVectorFieldColorModeDropdown;
        [SerializeField] private GUIDropdownInputLogic uiVectorFieldScaleModeDropdown;
        [SerializeField] private GUIDropdownInputLogic uiVectorFieldTransparencyModeDropdown;
        [SerializeField] private GUIFloatInputLogic    uiVectorFieldFixedTransparencySlider;
        [SerializeField] private GUIBoolInputLogic     uiVectorFieldDisplayOutsideOfRangeBool;

        [Header("UI-IsoSurface References")]
        [SerializeField] private GUIBoolInputLogic  uiIsoSurfaceEnabledToggle;
        [SerializeField] private GUIFloatInputLogic uiIsoSurfaceTransparency;
        [SerializeField] private GUIBoolInputLogic  uiIsoSurfaceUseMagnitudeColor;

        [Header("UI-Interpolation References")]
        [SerializeField] private GUIFloatInputLogic uiMaxMagnitude;
        [SerializeField] private GUIFloatInputLogic uiMagnitudeInterpolationExponent;

        [SerializeField] private GUIFloatInputLogic uiVoltageCenter;
        [SerializeField] private GUIFloatInputLogic uiVoltageRange;
        [SerializeField] private GUIFloatInputLogic uiVoltageInterpolationExponent;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering

            gridValuesComputeBuffer = new ComputeBuffer(VECTORFIELD_MAX_RESOLUTION * VECTORFIELD_MAX_RESOLUTION * VECTORFIELD_MAX_RESOLUTION, 4 * 4);

            // Add UI callbacks
            uiVectorFieldTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiVectorFieldFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        private void OnDestroy()
        {
            gridValuesComputeBuffer.Dispose();
        }

        // Calculate vector field values at grid-cell positions, updates compute buffer
        private void LateUpdate()
        {
            if (!uiVectorFieldEnabledToggle.GetValue()) return;

            VectorFieldInfos gridInfo = new VectorFieldInfos(uiVectorFieldResolutionSlider.GetValue(), uiVectorField3DModeToggle.GetValue());

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
            if (!uiVectorFieldEnabledToggle.GetValue() && !uiIsoSurfaceEnabledToggle.GetValue())
            {
                Graphics.Blit(source, destination);
                return;
            }

            float groundPotential = 0.0f;
            if (groundPin != null && groundPin.isActiveAndEnabled && groundPin.applyToVisualization)
            {
                groundPotential = ElectricField.Instance.GetPotential(groundPin.transform.position, true);
            }

            if (uiIsoSurfaceEnabledToggle.GetValue()) // Isosurface rendering
            {
                // Update shader values
                Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
                ElectricField.Instance.computeBuffers.SetUniformsForMaterial(isoSurfaceMaterial);
                isoSurfaceMaterial.SetBuffer("_VectorFieldValues", gridValuesComputeBuffer);
                isoSurfaceMaterial.SetMatrix("_InverseView", inverseView);

                var box = SimulationBox.Instance.Bounds;
                isoSurfaceMaterial.SetVector("_BoxMin", box.min);
                isoSurfaceMaterial.SetVector("_BoxMax", box.max);

                isoSurfaceMaterial.SetFloat("_Transparency", uiIsoSurfaceTransparency.GetValue());
                isoSurfaceMaterial.SetInt("_UseMagnitudeAsColor", uiIsoSurfaceUseMagnitudeColor.GetValue() ? 1 : 0);

                isoSurfaceMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitude.GetValue());
                isoSurfaceMaterial.SetFloat("_MagnitudeInterpolationExponent", uiMagnitudeInterpolationExponent.GetValue());
                isoSurfaceMaterial.SetFloat("_VoltageCenter", uiVoltageCenter.GetValue() + groundPotential);

                Graphics.Blit(source, destination, isoSurfaceMaterial);
            }
            else // Vectorfield rendering
            {
                VectorFieldInfos gridInfo = new VectorFieldInfos(uiVectorFieldResolutionSlider.GetValue(), uiVectorField3DModeToggle.GetValue());

                // Update shader values
                Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
                vectorFieldMaterial.SetBuffer("_VectorFieldValues", gridValuesComputeBuffer);
                vectorFieldMaterial.SetMatrix("_InverseView", inverseView);

                vectorFieldMaterial.SetVector("_BoxMin", gridInfo.gridMin);
                vectorFieldMaterial.SetInt("_FieldResolutionX", gridInfo.resolutionX);
                vectorFieldMaterial.SetInt("_FieldResolutionY", gridInfo.resolutionY);
                vectorFieldMaterial.SetInt("_FieldResolutionZ", gridInfo.resolutionZ);
                vectorFieldMaterial.SetFloat("_CellSize", gridInfo.cellSize);
                vectorFieldMaterial.SetFloat("_ArrowSize", uiVectorFieldArrowSizeSlider.GetValue());

                vectorFieldMaterial.SetInt("_ColorMode", uiVectorFieldColorModeDropdown.GetSelectedIndex());
                vectorFieldMaterial.SetInt("_ScaleMode", uiVectorFieldScaleModeDropdown.GetSelectedIndex());
                vectorFieldMaterial.SetInt("_TransparencyMode", uiVectorFieldTransparencyModeDropdown.GetSelectedIndex());
                vectorFieldMaterial.SetFloat("_FixedTransparencyValue", uiVectorFieldFixedTransparencySlider.GetValue());

                vectorFieldMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitude.GetValue());
                vectorFieldMaterial.SetFloat("_MagnitudeInterpolationExponent", uiMagnitudeInterpolationExponent.GetValue());

                vectorFieldMaterial.SetFloat("_VoltageCenter", uiVoltageCenter.GetValue() + groundPotential);
                vectorFieldMaterial.SetFloat("_VoltageRange", uiVoltageRange.GetValue());
                vectorFieldMaterial.SetFloat("_VoltageInterpolationExponent", uiVoltageInterpolationExponent.GetValue());
                vectorFieldMaterial.SetInt("_DisplayOutsideOfRangeBool", uiVectorFieldDisplayOutsideOfRangeBool.GetValue() ? 1 : 0);

                Graphics.Blit(source, destination, vectorFieldMaterial);
            }
        }
    }
}
