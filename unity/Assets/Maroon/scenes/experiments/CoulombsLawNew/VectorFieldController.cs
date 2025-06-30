using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Rendering;

namespace Maroon.Experiments.CoulombsLawNew
{
    enum ScalarDisplayMode
    {
        NONE,
        POTENTIAL,
        EFIELD_MAG
    }

    public class VectorFieldController : MonoBehaviour
    {
        [SerializeField] private float maxArrowWorldSize = 0.1f;
        [SerializeField] private float arrowSizeScale = 1.0f;

        // Rendering data
        [SerializeField] private Camera usedCamera;
        [SerializeField] private Material arrowMaterial;
        private Mesh arrowMesh = null;

        [Header("GUI references")]
        [SerializeField] private GuiBoolInputHandler uiEnabledToggle;
        [SerializeField] private GuiIntInputHandler  uiResolutionSlider;

        void Start()
        {
            arrowMesh = ArrowMeshCreator.CreateArrowMesh(0.2f, 1.3f, 0.5f, 12, 4);
        }

        private void LateUpdate()
        {
            if (!uiEnabledToggle.GetValue() || true) return;

            Bounds box = SimulationBox.Instance.Bounds;
            var efield = ElectricField.Instance;
            var in3DMode = CameraController.Instance.In3DMode;

            // Calculate grid-resolution and cell size (1 grid cell contains 1 arrow)
            float maxDimSize = Mathf.Max(box.size.x, box.size.y);
            if (in3DMode) {
                maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();

            int resolutionX = (int)(box.size.x / cellSize + 0.5f);
            int resolutionY = (int)(box.size.y / cellSize + 0.5f);
            int resolutionZ = (int)(box.size.z / cellSize + 0.5f);
            if (!in3DMode) resolutionZ = 1;

            var domainOrigin = box.min +
                (box.size - new Vector3(resolutionX, resolutionY, resolutionZ) * cellSize) / 2;

            // Generate draw calls for each arrow
            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    for (int z = 0; z < resolutionZ; z++)
                    {
                        // Get arrow position (At arrow center)
                        Vector3 arrowPos = domainOrigin + cellSize * new Vector3(x, y, z) + cellSize * Vector3.one / 2.0f;
                        if (!in3DMode) { arrowPos.z = box.center.z; }

                        // Calculate Electric Field value
                        var fieldValue = efield.GetFieldValue(arrowPos, true); // In [Newton/Coulomb]

                        // Calculate Arrow direction
                        if (!in3DMode) { fieldValue.z = 0.0f; }
                        if (fieldValue.sqrMagnitude <= 0.0f) // In case of no electric field, point upwards
                        {
                            fieldValue = Vector3.up;
                        }
                        Quaternion arrowRotation = Quaternion.FromToRotation(Vector3.forward, fieldValue.normalized);

                        // Calculate Arrow scale
                        Vector3 arrowScale = Vector3.one *  Mathf.Min(cellSize * arrowSizeScale, maxArrowWorldSize) / 2.0f; // Division by 2 because arrow-mesh is in range [-1,1]

                        // Generate draw call
                        if (!in3DMode) { arrowPos.z = box.max.z; } // TODO(MartinR): This should probably have other behavior in 2D
                        var propBlock = new MaterialPropertyBlock();
                        // propBlock.SetColor("_BaseColor", x % 2 == 0 ? Color.red : Color.blue);
                        propBlock.SetColor("_Color", x % 2 == 0 ? Color.red : Color.blue);
                        // propBlock.SetColor("_Albedo", x % 2 == 0 ? Color.red : Color.blue);
                        Graphics.DrawMesh(
                            arrowMesh, 
                            Matrix4x4.TRS(arrowPos, arrowRotation, arrowScale), 
                            arrowMaterial, 
                            0, Camera.main, 0, propBlock, false, false, true);
                    }
                }
            }
        }
    }
}
