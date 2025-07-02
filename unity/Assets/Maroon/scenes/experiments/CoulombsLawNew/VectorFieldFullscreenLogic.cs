using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VectorFieldFullscreenLogic : MonoBehaviour
    {
        public const int MAX_RESOLUTION = 40;
        public const int MAX_CHARGED_POINTS = 50;
        public const int MAX_CHARGED_RODS = 30;
        public const int MAX_CHARGED_PLANES = 30;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private ComputeShader gridValuesComputeShader;

        private ComputeBuffer gridValuesComputeBuffer;
        private ComputeBuffer chargedPointDataBuffer;
        private ComputeBuffer chargedRodPositionBuffer;
        private ComputeBuffer chargedRodDirectionBuffer;
        private ComputeBuffer chargedPlaneDataBuffer;
        private ComputeBuffer chargedPlaneChargeBuffer;

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

            gridValuesComputeBuffer = new ComputeBuffer(MAX_RESOLUTION * MAX_RESOLUTION * MAX_RESOLUTION, 4 * 4);

            chargedPointDataBuffer    = new ComputeBuffer(MAX_CHARGED_POINTS, 4 * 4);
            chargedRodPositionBuffer  = new ComputeBuffer(MAX_CHARGED_RODS,   4 * 4);
            chargedRodDirectionBuffer = new ComputeBuffer(MAX_CHARGED_RODS,   4 * 4);
            chargedPlaneDataBuffer    = new ComputeBuffer(MAX_CHARGED_PLANES, 4 * 4);
            chargedPlaneChargeBuffer  = new ComputeBuffer(MAX_CHARGED_PLANES, 4);

            uiTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        private void OnDestroy()
        {
            gridValuesComputeBuffer.Dispose();
            chargedPointDataBuffer.Dispose();
            chargedRodPositionBuffer.Dispose();
            chargedRodDirectionBuffer.Dispose();
            chargedPlaneDataBuffer.Dispose();
            chargedPlaneChargeBuffer.Dispose();
        }

        private void LateUpdate()
        {
            // Calculate vector field values at grid-cell positions, and update compute buffer

            var efield = ElectricField.Instance;
            var box = SimulationBox.Instance.Bounds;
            int resolution = uiResolutionSlider.GetValue();

            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();
            var domainOrigin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

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

            // Run compute shader to update vector field values
            List<Vector4> chargedPointData = new List<Vector4>();
            foreach (var pointCharge in ElectricField.Instance.chargedPoints)
            {
                if (chargedPointData.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                chargedPointData.Add(packedInfo);
            }
            chargedPointDataBuffer.SetData<Vector4>(chargedPointData);

            List<Vector4> chargedRodPositions = new List<Vector4>();
            List<Vector4> chargedRodDirections = new List<Vector4>();
            foreach (var chargedRod in ElectricField.Instance.chargedRods)
            {
                if (chargedRodPositions.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Vector4 packedPos = new Vector4(pos.x, pos.y, pos.z, chargedRod.GetChargeDensity());
                Vector4 packedDir = new Vector4(dir.x, dir.y, dir.z, 0);
                chargedRodPositions.Add(packedPos);
                chargedRodDirections.Add(packedDir);
            }
            chargedRodPositionBuffer.SetData<Vector4>(chargedRodPositions);
            chargedRodDirectionBuffer.SetData<Vector4>(chargedRodDirections);

            // Get charged plane packed data
            List<Vector4> chargedPlaneEquations = new List<Vector4>();
            List<float> chargedPlaneChargeDensities = new List<float>();
            foreach (var chargedPlane in ElectricField.Instance.chargedPlanes)
            {
                if (chargedPlaneEquations.Count + 1 >= MAX_CHARGED_PLANES) break;
                var normal = chargedPlane.GetNormal();
                Vector4 equation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, chargedPlane.transform.position));
                chargedPlaneEquations.Add(equation);
                chargedPlaneChargeDensities.Add(chargedPlane.GetChargeDensity());
            }
            chargedPlaneDataBuffer.SetData<Vector4>(chargedPlaneEquations);
            chargedPlaneChargeBuffer.SetData<float>(chargedPlaneChargeDensities);



            // Set compute shader uniform values
            int kernelIndex = gridValuesComputeShader.FindKernel("CSMain");
            gridValuesComputeShader.SetBuffer(kernelIndex, "_VectorFieldBuffer", gridValuesComputeBuffer);
            gridValuesComputeShader.SetInt("_VectorFieldResolution", resolution);

            gridValuesComputeShader.SetInt("_ChargedPointCount", chargedPointData.Count);
            gridValuesComputeShader.SetInt("_ChargedRodCount",   chargedRodPositions.Count);
            gridValuesComputeShader.SetInt("_ChargedPlaneCount", chargedPlaneEquations.Count);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_ChargedPointData", chargedPointDataBuffer);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_ChargedRodPositions", chargedRodPositionBuffer);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_ChargedRodDirections", chargedRodDirectionBuffer);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_ChargedPlaneEquations", chargedPlaneDataBuffer);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_ChargedPlaneChargeDensities", chargedPlaneChargeBuffer);

            Vector3 min = SimulationBox.Instance.Bounds.min;
            gridValuesComputeShader.SetVector("_GridMin", new Vector4(min.x, min.y, min.z, 0.0f));
            gridValuesComputeShader.SetFloat("_CellSize", cellSize);
            gridValuesComputeShader.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            gridValuesComputeShader.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);

            // Launch compute shader
            uint threadGroupX, threadGroupY, threadGroupZ;
            gridValuesComputeShader.GetKernelThreadGroupSizes(kernelIndex, out threadGroupX, out threadGroupY, out threadGroupZ);
            int requiredGroups = ((resolution * resolution * resolution) / (int)(threadGroupX * threadGroupY * threadGroupZ)) + 1;
            gridValuesComputeShader.Dispatch(kernelIndex, requiredGroups, 1, 1);

            // Vector4[] values = new Vector4[resolution * resolution * resolution];
            // vectorFieldValuesBuffer.GetData(values);

            // for (int i = 0; i < 10; i++)
            // {
            //     Debug.Log("Value.x: " + values[i].x + " y: " + values[i].y);
            // }
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
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();
            var domainOrigin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

            // Update shader values
            Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
            vectorFieldMaterial.SetBuffer("_VectorFieldValues", gridValuesComputeBuffer);
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
