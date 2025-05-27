using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace Maroon.Experiments.CoulombsLaw
{
    enum ScalarDisplayMode
    {
        NONE,
        POTENTIAL,
        EFIELD_MAG
    }

    public class VectorFieldController : MonoBehaviour
    {
        private Mesh arrowMesh = null;
        private GameObject[] vectorFieldArrows = null;

        [SerializeField]
        private Material vectorMaterial = null;
        [SerializeField]
        private Transform maxBoundary = null;
        [SerializeField]
        private Transform minBoundary = null;
        [SerializeField]
        private float padding = 0.0f;
        [SerializeField]
        private Maroon.Physics.Electromagnetism.EField efield = null;
        private bool _visible = true;
        public bool Visible { get { return _visible;  } set { _visible = value; GenerateVectorFieldArrows(); } }

        private int _resolution = 10;
        [SerializeField]
        private float maxArrowWorldSize = 0.1f;
        [Range(0.1f, 1.3f)]
        [SerializeField]
        private float arrowSizeScale = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            arrowMesh = ArrowMeshCreator.CreateArrowMesh(0.2f, 1.3f, 0.5f, 12, 4);
            GenerateVectorFieldArrows();
        }

        private void GenerateVectorFieldArrows()
        {
            // Early exit if vector field is disabled
            if (!_visible) {
                if (vectorFieldArrows != null)
                {
                    foreach (var arrow in vectorFieldArrows)
                    {
                        GameObject.Destroy(arrow);
                    }
                    vectorFieldArrows = null;
                }
                return;
            }

            // Remove previous Arrow-Objects if resolution size changed
            int wantedArrowCount = _resolution * _resolution;
            if (vectorFieldArrows != null && vectorFieldArrows.Length != wantedArrowCount)
            {
                foreach (var arrow in vectorFieldArrows) {
                    GameObject.Destroy(arrow);
                }
                vectorFieldArrows = null;
            }

            // Create new Arrow-Objects if necessary
            if (vectorFieldArrows == null)
            {
                vectorFieldArrows = new GameObject[wantedArrowCount];
                for (int i = 0; i < vectorFieldArrows.Length; i++) 
                {
                    // Instanciate and initialize object
                    var newArrow = new GameObject("VectorFieldArrow");
                    newArrow.transform.parent = transform;
                    var meshRenderer = newArrow.AddComponent<MeshRenderer>();
                    meshRenderer.material = vectorMaterial;
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    var meshFilter = newArrow.AddComponent<MeshFilter>();
                    meshFilter.mesh = arrowMesh;

                    vectorFieldArrows[i] = newArrow;
                }
            }

            // Set position for each Arrow
            var domainSize = maxBoundary.transform.position - minBoundary.transform.position - padding * 2.0f * new Vector3(1, 1, 1);
            var minSideLength = Mathf.Min(domainSize.x, domainSize.y);
            var domainOrigin = minBoundary.transform.position + padding * new Vector3(1, 1, 0);
            float cellSideLength = minSideLength / _resolution;
            domainOrigin.z -= cellSideLength / 2.0f;

            for (int i = 0; i < vectorFieldArrows.Length; i++)
            {
                var arrow = vectorFieldArrows[i];

                int cellX = i % _resolution;
                int cellY = (i / _resolution) % _resolution;
                int cellZ = i / (_resolution * _resolution);
                Vector3 offset = new Vector3(
                    cellSideLength * cellX + cellSideLength / 2.0f,
                    cellSideLength * cellY + cellSideLength / 2.0f,
                    cellSideLength * cellZ + cellSideLength / 2.0f
                );
                arrow.transform.position = domainOrigin + offset;
            }
        }
        
        public void SetResolutionFromFloat(float value)
        {
            _resolution = (int)value;
            GenerateVectorFieldArrows();
        }

        void Update()
        {
            if (vectorFieldArrows == null || !_visible) return;

            // Update arrow properties (Direction, color, scale)
            var domainSize = maxBoundary.transform.position - minBoundary.transform.position - padding * 2.0f * new Vector3(1, 1, 1);
            var minSideLength = Mathf.Min(domainSize.x, domainSize.y);
            float cellSideLength = minSideLength / _resolution;

            var coordSystem = Maroon.GlobalEntities.CoordSystemHandler.Instance;
            foreach (var arrow in vectorFieldArrows)
            {
                // Calculate Electric Field and Potential value
                Vector3 arrowPos = arrow.transform.position;
                arrowPos.z = 0.0f; // 2D Mode
                var fieldValue = efield.get(coordSystem.GetSystemPosition(arrowPos), gameObject); // In [Newton/Coulomb]

                // Set Arrow direction
                float len = fieldValue.sqrMagnitude;
                Vector3 arrowDirection = fieldValue.normalized;
                if (fieldValue.sqrMagnitude <= 0.0f) // In case of no electric field, point upwards
                {
                    arrowDirection = Vector3.up;
                }
                arrow.transform.rotation = Quaternion.FromToRotation(Vector3.forward, arrowDirection);

                // Set Arrow scale
                float scale = Mathf.Min(cellSideLength * arrowSizeScale, maxArrowWorldSize) / 2.0f; // Division by 2 because arrow-mesh is in range [-1,1]
                arrow.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
