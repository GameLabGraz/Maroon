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
        // Note: RenderTexture size is currently 256x256 = 65.536,
        //  so a VectorField with 40*40*40 = 64.000 still fits into the texture
        //  The TEXTURE_RESOLUTION needs to be manually updated if the limits are changed...
        public const int VECTORFIELD_MAX_RESOLUTION = 40;
        public const int TEXTURE_RESOLUTION = 256;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private Material isoSurfaceMaterial;
        [SerializeField] private Material vectorFieldGridCalculationMaterial;
        [SerializeField] private GroundPinLogic groundPin;

        // This texture stores the vector value and potential of the vector field
        //  evaluated at the grid-positions. See VectorFieldGridCalculateShader for how the packing is done
        private CustomRenderTexture gridValuesTexture;

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

        [SerializeField] private GUIFloatInputLogic uiVoltageRange;
        [SerializeField] private GUIFloatInputLogic uiVoltageInterpolationExponent;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering

            gridValuesTexture = new CustomRenderTexture(
                TEXTURE_RESOLUTION, TEXTURE_RESOLUTION, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            gridValuesTexture.doubleBuffered = false;
            gridValuesTexture.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
            gridValuesTexture.material = vectorFieldGridCalculationMaterial;
            gridValuesTexture.updateMode = CustomRenderTextureUpdateMode.OnDemand;

            uiVectorFieldResolutionSlider.SetMinMax(4, VECTORFIELD_MAX_RESOLUTION);

            // Add UI callbacks
            uiVectorFieldTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiVectorFieldFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        // Calculate vector field values at grid-cell positions, stores results it in gridValuesTexture
        private void LateUpdate()
        {
            if (!uiVectorFieldEnabledToggle.GetValue()) return;

            VectorFieldInfos gridInfo = new VectorFieldInfos(uiVectorFieldResolutionSlider.GetValue(), uiVectorField3DModeToggle.GetValue());

            // Calculate Vector-Field Values using CustomRenderTexture
            var efield = ElectricField.Instance;
            vectorFieldGridCalculationMaterial.SetInt("_GridResolutionX", gridInfo.resolutionX);
            vectorFieldGridCalculationMaterial.SetInt("_GridResolutionY", gridInfo.resolutionY);
            vectorFieldGridCalculationMaterial.SetInt("_GridResolutionZ", gridInfo.resolutionZ);
            vectorFieldGridCalculationMaterial.SetVector("_GridMin", new Vector4(gridInfo.gridMin.x, gridInfo.gridMin.y, gridInfo.gridMin.z, 0.0f));
            vectorFieldGridCalculationMaterial.SetFloat("_CellSize", gridInfo.cellSize);
            efield.electricFieldPackedGPUData.SetUniformsForMaterial(vectorFieldGridCalculationMaterial);

            gridValuesTexture.Update();

            // CPU-Logic for buffer-values, maybe we want this if CustomRenderTexture is not supported?
            //      Note that for this to work CustomRenderTexture should be changed to Texture2D type
            // var efield = ElectricField.Instance;
            // Unity.Collections.NativeArray<Color> rawTextureData = gridValuesTexture.GetRawTextureData<Color>();
            // for (int x = 0; x < gridInfo.resolutionX; x++)
            // {
            //     for (int y = 0; y < gridInfo.resolutionY; y++)
            //     {
            //         for (int z = 0; z < gridInfo.resolutionZ; z++)
            //         {
            //             int linearIndex = x + y * gridInfo.resolutionX + z * gridInfo.resolutionX * gridInfo.resolutionY;

            //             // Get arrow position (At arrow center)
            //             Vector3 arrowPos = gridInfo.gridMin + gridInfo.cellSize * (new Vector3(x, y, z) + Vector3.one * 0.5f);

            //             // Calculate Electric Field value
            //             var fieldValue = efield.GetFieldValue(arrowPos, true); // In [Newton/Coulomb]
            //             var potential = efield.GetPotential(arrowPos, true); // In Volt

            //             // Pack values and store in texture
            //             Color color = new Color(fieldValue.x, fieldValue.y, fieldValue.z, potential);
            //             rawTextureData[linearIndex] = color;
            //         }
            //     }
            // }
            // // Upload texture data to GPU (Unity keeps Texture2D data in ram and in vram, and only uploads data on Apply)
            // //      Note: We're uploading 1MB of texture data each frame, given a PCIe 3.0 connection (2010 technology)
            // //          @60FPS we have 266MB of data to upload per frame, so 1MB per frame should be fine on a laptop/desktop
            // //          Not sure about moblile devices, to improve performance we could only upload parts of the texture that are
            // //          actually changed.
            // gridValuesTexture.Apply();
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
            if (groundPin != null && groundPin.isActiveAndEnabled)
            {
                groundPotential = ElectricField.Instance.GetPotential(groundPin.transform.position, true);
            }

            if (uiIsoSurfaceEnabledToggle.GetValue()) // Isosurface rendering
            {
                // Update shader values
                Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
                ElectricField.Instance.electricFieldPackedGPUData.SetUniformsForMaterial(isoSurfaceMaterial);
                isoSurfaceMaterial.SetMatrix("_InverseView", inverseView);

                var box = SimulationBox.Instance.Bounds;
                isoSurfaceMaterial.SetVector("_BoxMin", box.min);
                isoSurfaceMaterial.SetVector("_BoxMax", box.max);

                isoSurfaceMaterial.SetFloat("_Transparency", uiIsoSurfaceTransparency.GetValue());
                isoSurfaceMaterial.SetInt("_UseMagnitudeAsColor", uiIsoSurfaceUseMagnitudeColor.GetValue() ? 1 : 0);

                isoSurfaceMaterial.SetFloat("_MaxMagnitude", uiMaxMagnitude.GetValue());
                isoSurfaceMaterial.SetFloat("_MagnitudeInterpolationExponent", uiMagnitudeInterpolationExponent.GetValue());
                isoSurfaceMaterial.SetFloat("_VoltageCenter", groundPotential);

                Graphics.Blit(source, destination, isoSurfaceMaterial);
            }
            else if (uiVectorFieldEnabledToggle.GetValue()) // Vectorfield rendering
            {
                VectorFieldInfos gridInfo = new VectorFieldInfos(uiVectorFieldResolutionSlider.GetValue(), uiVectorField3DModeToggle.GetValue());

                // Update shader values
                Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
                // vectorFieldMaterial.SetBuffer("_VectorFieldValues", gridValuesComputeBuffer);
                vectorFieldMaterial.SetTexture("_GridValuesTexture", gridValuesTexture);
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

                vectorFieldMaterial.SetFloat("_VoltageCenter", groundPotential);
                vectorFieldMaterial.SetFloat("_VoltageRange", uiVoltageRange.GetValue());
                vectorFieldMaterial.SetFloat("_VoltageInterpolationExponent", uiVoltageInterpolationExponent.GetValue());
                vectorFieldMaterial.SetInt("_DisplayOutsideOfRangeBool", uiVectorFieldDisplayOutsideOfRangeBool.GetValue() ? 1 : 0);

                Graphics.Blit(source, destination, vectorFieldMaterial);
            }
        }
    }
}
