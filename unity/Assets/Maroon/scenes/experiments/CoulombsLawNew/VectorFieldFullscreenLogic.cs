using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedObjectComputeBuffers
    {
        public const int MAX_CHARGED_POINTS = 50;
        public const int MAX_CHARGED_RODS = 30;
        public const int MAX_CHARGED_PLANES = 30;

        public ComputeBuffer chargedPointData; // xyz position, w is charge
        public ComputeBuffer chargedRodPositions; // xyz position, w is charge
        public ComputeBuffer chargedRodDirections; // xyz direction, w unused
        public ComputeBuffer chargedPlaneEquations; // xyz normal, w is negative distance from plane to origin
        public ComputeBuffer chargedPlaneChargeDensities; // Note: this is a float buffer, all other are float4

        List<Vector4> cpuChargedPointData = new List<Vector4>();
        List<Vector4> cpuChargedRodPositions = new List<Vector4>();
        List<Vector4> cpuChargedRodDirections = new List<Vector4>();
        List<Vector4> cpuChargedPlaneEquations = new List<Vector4>();
        List<float> cpuChargedPlaneChargeDensities = new List<float>();

        private int lastUpdateFrame = -1;

        public ChargedObjectComputeBuffers()
        {
            chargedPointData = new ComputeBuffer(MAX_CHARGED_POINTS, 4 * 4);
            chargedRodPositions = new ComputeBuffer(MAX_CHARGED_RODS, 4 * 4);
            chargedRodDirections = new ComputeBuffer(MAX_CHARGED_RODS, 4 * 4);
            chargedPlaneEquations = new ComputeBuffer(MAX_CHARGED_PLANES, 4 * 4);
            chargedPlaneChargeDensities = new ComputeBuffer(MAX_CHARGED_PLANES, 4);
        }

        public void DisposeBuffers()
        {
            chargedPointData.Dispose();
            chargedRodPositions.Dispose();
            chargedRodDirections.Dispose();
            chargedPlaneEquations.Dispose();
            chargedPlaneChargeDensities.Dispose();
        }

        private void UpdateBuffersForCurrentFrame()
        {
            if (lastUpdateFrame == UnityEngine.Time.frameCount) return;
            lastUpdateFrame = UnityEngine.Time.frameCount;

            cpuChargedPointData.Clear();
            cpuChargedRodPositions.Clear();
            cpuChargedRodDirections.Clear();
            cpuChargedPlaneEquations.Clear();
            cpuChargedPlaneChargeDensities.Clear();

            // Update charged object buffers
            foreach (var pointCharge in ElectricField.Instance.chargedPoints)
            {
                if (cpuChargedPointData.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                cpuChargedPointData.Add(packedInfo);
            }
            chargedPointData.SetData<Vector4>(cpuChargedPointData);

            foreach (var chargedRod in ElectricField.Instance.chargedRods)
            {
                if (cpuChargedRodPositions.Count + 1 >= MAX_CHARGED_POINTS) break;
                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Vector4 packedPos = new Vector4(pos.x, pos.y, pos.z, chargedRod.GetChargeDensity());
                Vector4 packedDir = new Vector4(dir.x, dir.y, dir.z, 0);
                cpuChargedRodPositions.Add(packedPos);
                cpuChargedRodDirections.Add(packedDir);
            }
            chargedRodPositions.SetData<Vector4>(cpuChargedRodPositions);
            chargedRodDirections.SetData<Vector4>(cpuChargedRodDirections);

            // Get charged plane packed data
            foreach (var chargedPlane in ElectricField.Instance.chargedPlanes)
            {
                if (cpuChargedPlaneEquations.Count + 1 >= MAX_CHARGED_PLANES) break;
                var normal = chargedPlane.GetNormal();
                Vector4 equation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, chargedPlane.transform.position));
                cpuChargedPlaneEquations.Add(equation);
                cpuChargedPlaneChargeDensities.Add(chargedPlane.GetChargeDensity());
            }
            chargedPlaneEquations.SetData<Vector4>(cpuChargedPlaneEquations);
            chargedPlaneChargeDensities.SetData<float>(cpuChargedPlaneChargeDensities);
        }

        // Shader should include "ShaderUtils.cginc" for this to work
        public void SetUniformsForComputeShader(ComputeShader computeShader, int kernelIndex)
        {
            UpdateBuffersForCurrentFrame();

            computeShader.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            computeShader.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            computeShader.SetInt("_ChargedPointCount", cpuChargedPointData.Count);
            computeShader.SetInt("_ChargedRodCount", cpuChargedRodPositions.Count);
            computeShader.SetInt("_ChargedPlaneCount", cpuChargedPlaneEquations.Count);
            computeShader.SetBuffer(kernelIndex, "_ChargedPointData", chargedPointData);
            computeShader.SetBuffer(kernelIndex, "_ChargedRodPositions", chargedRodPositions);
            computeShader.SetBuffer(kernelIndex, "_ChargedRodDirections", chargedRodDirections);
            computeShader.SetBuffer(kernelIndex, "_ChargedPlaneEquations", chargedPlaneEquations);
            computeShader.SetBuffer(kernelIndex, "_ChargedPlaneChargeDensities", chargedPlaneChargeDensities);
        }

        public void SetUniformsForMaterial(Material material)
        {
            UpdateBuffersForCurrentFrame();

            material.SetFloat("_PointChargeMinDist", ChargedPoint.RADIUS);
            material.SetFloat("_ChargedRodMinDist", ChargedRod.RADIUS);
            material.SetInt("_ChargedPointCount", cpuChargedPointData.Count);
            material.SetInt("_ChargedRodCount", cpuChargedRodPositions.Count);
            material.SetInt("_ChargedPlaneCount", cpuChargedPlaneEquations.Count);
            material.SetBuffer("_ChargedPointData", chargedPointData);
            material.SetBuffer("_ChargedRodPositions", chargedRodPositions);
            material.SetBuffer("_ChargedRodDirections", chargedRodDirections);
            material.SetBuffer("_ChargedPlaneEquations", chargedPlaneEquations);
            material.SetBuffer("_ChargedPlaneChargeDensities", chargedPlaneChargeDensities);
        }
    }

    public class VectorFieldFullscreenLogic : MonoBehaviour
    {
        public const int MAX_RESOLUTION = 40;

        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private ComputeShader gridValuesComputeShader;

        private ComputeBuffer gridValuesComputeBuffer;
        public ChargedObjectComputeBuffers chargedObjectComputeBuffers;

        [Header("UI-References")]
        [SerializeField] private GuiBoolInputHandler uiEnabledToggle;
        [SerializeField] private GuiIntInputHandler uiResolutionSlider;
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
            chargedObjectComputeBuffers = new ChargedObjectComputeBuffers();

            uiTransparencyModeDropdown.OnValueChanged.AddListener((int dropdownValue) =>
            {
                uiFixedTransparencySlider.gameObject.SetActive(dropdownValue == 3);
            });
        }

        private void OnDestroy()
        {
            gridValuesComputeBuffer.Dispose();
            chargedObjectComputeBuffers.DisposeBuffers();
        }

        private void LateUpdate()
        {
            // Calculate vector field values at grid-cell positions, and update compute buffer
            var box = SimulationBox.Instance.Bounds;
            int resolution = uiResolutionSlider.GetValue();
            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            float cellSize = maxDimSize / resolution;
            Vector3 min = SimulationBox.Instance.Bounds.min;

            // Set compute shader uniform values
            int kernelIndex = gridValuesComputeShader.FindKernel("CSMain");
            chargedObjectComputeBuffers.SetUniformsForComputeShader(gridValuesComputeShader, kernelIndex);
            gridValuesComputeShader.SetBuffer(kernelIndex, "_VectorFieldBuffer", gridValuesComputeBuffer);
            gridValuesComputeShader.SetInt("_VectorFieldResolution", resolution);
            gridValuesComputeShader.SetVector("_GridMin", new Vector4(min.x, min.y, min.z, 0.0f));
            gridValuesComputeShader.SetFloat("_CellSize", cellSize);

            // Launch compute shader
            uint threadGroupX, threadGroupY, threadGroupZ;
            gridValuesComputeShader.GetKernelThreadGroupSizes(kernelIndex, out threadGroupX, out threadGroupY, out threadGroupZ);
            int requiredGroups = ((resolution * resolution * resolution) / (int)(threadGroupX * threadGroupY * threadGroupZ)) + 1;
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
            vectorFieldMaterial.SetFloat("_VoltageRange", uiVoltageRangeKV.GetValue() * 1000.0f);
            vectorFieldMaterial.SetFloat("_VoltageInterpolationExponent", uiVoltageInterpolationExponent.GetValue());
            vectorFieldMaterial.SetInt("_DisplayOutsideOfRangeBool", uiDisplayOutsideOfRangeBool.GetValue() ? 1 : 0);

            Graphics.Blit(source, destination, vectorFieldMaterial);
        }
    }
}
