using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

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
        [SerializeField] private Material vectorMaterial;
        [SerializeField] private float maxArrowWorldSize = 0.1f;
        [SerializeField] private GameObject arrowContainerObject;
        [SerializeField] private float arrowSizeScale = 1.0f;

        [Header("GUI references")]
        [SerializeField] private GuiBoolInputHandler uiEnabledToggle;
        [SerializeField] private GuiIntInputHandler  uiResolutionSlider;

        private Mesh arrowMesh = null;
        private GameObject[] vectorFieldArrows = new GameObject[0];

        void Awake()
        {
            arrowMesh = ArrowMeshCreator.CreateArrowMesh(0.2f, 1.3f, 0.5f, 12, 4);

            SimulationBox.Instance.OnBoundsChanged.AddListener((_unused) => GenerateVectorFieldArrowGrid());
            CameraController.Instance.OnCameraModeChanged.AddListener(() => GenerateVectorFieldArrowGrid());
            uiResolutionSlider.OnValueChanged.AddListener((int _unused) => GenerateVectorFieldArrowGrid());
            uiEnabledToggle.OnValueChanged.AddListener((bool _unused) => GenerateVectorFieldArrowGrid());
            GenerateVectorFieldArrowGrid();
        }

        private int IntMin(int a, int b) { return a < b ? a : b; }

        private void GenerateVectorFieldArrowGrid()
        {
            Bounds box = SimulationBox.Instance.Bounds;
            var in3DMode = CameraController.Instance.In3DMode;

            var enabled = uiEnabledToggle.GetValue();
            arrowContainerObject.SetActive(enabled);
            if (!enabled) return; 

            // Calculate grid-resolution and cell size (1 grid cell contains 1 arrow)
            float maxDimSize = Mathf.Max(box.size.x, box.size.y);
            if (in3DMode) {
                maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();

            // Note(MartinR): The +0.2f is used to round up some values that would otherwise get rounded down
            //      because of floating point inaccuracies
            int resolutionX = (int)(box.size.x / cellSize + 0.2f);
            int resolutionY = (int)(box.size.y / cellSize + 0.2f);
            int resolutionZ = (int)(box.size.z / cellSize + 0.2f);
            if (!in3DMode) resolutionZ = 1;

            // Resize arrow-array
            int wantedArrowCount = resolutionX * resolutionY * resolutionZ;
            if (wantedArrowCount != vectorFieldArrows.Length)
            {
                var newArrowsArray = new GameObject[wantedArrowCount];
                // Copy references to previous gameobjects
                for (int i = 0; i < IntMin(wantedArrowCount, vectorFieldArrows.Length); i++)
                {
                    newArrowsArray[i] = vectorFieldArrows[i];
                }

                // Create new Arrow-GameObjects if we need more than we had previously
                for (int i = vectorFieldArrows.Length; i < wantedArrowCount; i++)
                {
                    var newArrow = new GameObject("VectorFieldArrow");
                    newArrow.transform.SetParent(arrowContainerObject.transform);
                    var meshRenderer = newArrow.AddComponent<MeshRenderer>();
                    meshRenderer.material = vectorMaterial;
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    var meshFilter = newArrow.AddComponent<MeshFilter>();
                    meshFilter.mesh = arrowMesh;

                    newArrowsArray[i] = newArrow;
                }

                // Destroy old which aren't needed anymore
                for (int i = wantedArrowCount; i < vectorFieldArrows.Length; i++)
                {
                    GameObject.Destroy(vectorFieldArrows[i]);
                }

                vectorFieldArrows = newArrowsArray;
            }

            // Set position for each Arrow
            var domainOrigin = box.min +
                (box.size - new Vector3(resolutionX, resolutionY, resolutionZ) * cellSize) / 2;
            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    for (int z = 0; z < resolutionZ; z++)
                    {
                        Vector3 pos = domainOrigin + cellSize * new Vector3(x, y, z) + cellSize * Vector3.one / 2.0f;
                        if (!in3DMode)
                        {
                            pos.z = box.max.z;
                        }
                        vectorFieldArrows[x + y * resolutionX + z * (resolutionX * resolutionY)].transform.position = pos;
                    }
                }
            }
        }
        
        void LateUpdate()
        {
            if (!uiEnabledToggle.GetValue()) return;
            var in3DMode = CameraController.Instance.In3DMode;

            // Calculate grid-resolution and cell size (1 grid cell contains 1 arrow)
            var box = SimulationBox.Instance.Bounds;
            float maxDimSize = Mathf.Max(box.size.x, box.size.y);
            if (in3DMode) {
                maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();

            var efield = ElectricField.Instance;
            foreach (var arrow in vectorFieldArrows)
            {
                // Calculate Electric Field and Potential value
                Vector3 arrowPos = arrow.transform.position;
                if (!in3DMode) { arrowPos.z = box.center.z; } // TODO(MartinR): This should probably have other behavior in 2D
                var fieldValue = efield.GetFieldValue(arrowPos, true); // In [Newton/Coulomb]

                // Set Arrow direction
                if (!in3DMode) { fieldValue.z = 0.0f; }
                if (fieldValue.sqrMagnitude <= 0.0f) // In case of no electric field, point upwards
                {
                    fieldValue = Vector3.up;
                }
                arrow.transform.rotation = Quaternion.FromToRotation(Vector3.forward, fieldValue.normalized);

                // Set Arrow scale
                float scale = Mathf.Min(cellSize * arrowSizeScale, maxArrowWorldSize) / 2.0f; // Division by 2 because arrow-mesh is in range [-1,1]
                arrow.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
