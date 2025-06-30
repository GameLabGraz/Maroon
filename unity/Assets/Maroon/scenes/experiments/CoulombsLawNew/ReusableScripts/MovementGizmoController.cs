using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MovementGizmoController : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] private float offsetFromObject = 0.1f;
        [SerializeField] [Range(0, 1)] private float arrowSize = 0.1f;
        [SerializeField] private LineRenderer _lineRenderer;

        private MovementGizmoArrow[] arrows = new MovementGizmoArrow[6]; // See Awake to know which arrows are which

        private Rigidbody _rigidbodyOfSelected = null;
        private Vector3 _objectPositionAtDragStart;
        private Vector3 _offsetAtDragStart;
        private bool _rigidbodyWasKinematicAtDragStart;

        private void InitializeArrows()
        {
            // Find references to arrows
            arrows[0] = transform.Find("PositiveX").GetComponent<MovementGizmoArrow>();
            arrows[1] = transform.Find("PositiveY").GetComponent<MovementGizmoArrow>();
            arrows[2] = transform.Find("PositiveZ").GetComponent<MovementGizmoArrow>();
            arrows[3] = transform.Find("NegativeX").GetComponent<MovementGizmoArrow>();
            arrows[4] = transform.Find("NegativeY").GetComponent<MovementGizmoArrow>();
            arrows[5] = transform.Find("NegativeZ").GetComponent<MovementGizmoArrow>();

            // Set arrow meshes and color
            Mesh arrowMesh = ArrowMeshCreator.CreateArrowMesh(.2f, 1.2f, .4f, 16, 4);
            foreach (var arrow in arrows)
            {
                var meshFilter = arrow.GetComponent<MeshFilter>();
                meshFilter.mesh = arrowMesh;
                Color color = Color.black;
                color[arrow.dimension] = 1.0f;
                arrow.GetComponent<MeshRenderer>().material.color = color;
            }

            // Note: ArrowMesh points towards positive z (forward) by default
            arrows[0].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
            arrows[1].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
            arrows[2].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.forward);
            arrows[3].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.left);
            arrows[4].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.down);
            arrows[5].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.back);

            // Initial Positioning (So that it looks nice in edit-mode)
            for (int i = 0; i < arrows.Length; i++)
            {
                var arrow = arrows[i];
                Vector3 offsetDir = Vector3.zero;
                offsetDir[i % 3] = i >= 3 ? -1 : 1; // See Awake for how arrow-indices relate to dimensions

                arrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
                arrow.transform.localPosition = offsetDir * (arrowSize + offsetFromObject);
            }
        }

        // Note(MartinR): Uncomment if you want to play around/see gizmo in editor, but this causes some warnings to appear
        // private void OnValidate() { InitializeArrows(); }

        private void Awake()
        {
            InitializeArrows();
            UpdateArrowsDependingOnSelection();
            SelectionSystem.Instance.OnSelectionChanged.AddListener((SelectableObject selected) => 
            {
                _rigidbodyOfSelected = null;
                if (selected != null)
                {
                    _rigidbodyOfSelected = selected.GetComponent<Rigidbody>();
                }

                UpdateArrowsDependingOnSelection();
            });
            CameraController.Instance.OnCameraModeChanged.AddListener(() => UpdateArrowsDependingOnSelection());

            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
            }
        }

        public void UpdateArrowsDependingOnSelection()
        {
            // Hide/Show arrows depending on selection
            var selected = SelectionSystem.Instance.GetSelectedObject();
            bool arrowsActive = selected != null && selected.enableMovementGizmo;
            bool in3D = CameraController.Instance.In3DMode;
            foreach (var arrow in arrows) {
                arrow.gameObject.SetActive(arrowsActive && !(arrow.dimension == 2 && !in3D)); 
            }
            if (!arrowsActive) return;

            // Position arrows around selected object
            for (int i = 0; i < arrows.Length; i++)
            {
                var arrow = arrows[i];
                Vector3 offsetDir = Vector3.zero;
                offsetDir[i % 3] = i >= 3 ? -1 : 1; // See Awake for how arrow-indices relate to dimensions

                // TODO(MartinR): Scale arrows depending on distance to camera, so far away objects still have visible gizmo
                arrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
                arrow.transform.position = 
                    selected.transform.position + 
                    offsetDir * (selected.boundingRadius + offsetFromObject + arrowSize);
            }
        }

        private void LateUpdate() { UpdateArrowsDependingOnSelection(); }



        // Drag-and-Drop Logic starts here

        // It is assumed that ray directions are normalized
        private static Vector3 ClosestPointOnRayToOtherRay(Ray ray, Ray other)
        {
            Vector3 a = ray.direction;
            Vector3 b = other.direction;
            Vector3 c = other.origin - ray.origin;

            float t = 
                (-Vector3.Dot(a, b) * Vector3.Dot(b, c) + Vector3.Dot(a, c) * Vector3.Dot(b, b)) /
                (Vector3.Dot(a, a) * Vector3.Dot(b, b) - Vector3.Dot(a, b) * Vector3.Dot(a, b));

            return ray.GetPoint(t);
        }

        private Vector3 ClosestPointOnMovementAxisToMouse(int axis)
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 movementDir = Vector3.zero;
            movementDir[axis] = 1.0f;
            var movementRay = new Ray(_objectPositionAtDragStart, movementDir);

            // If mouse ray and axis ray are almost parallel, return initial position (disallow movement)
            if (1.0f - Mathf.Abs(Vector3.Dot(movementRay.direction, mouseRay.direction)) < 0.00001f)
            {
                return _objectPositionAtDragStart;
            }

            return ClosestPointOnRayToOtherRay(movementRay, mouseRay);
        }

        private bool dragActive = false;
        public void OnArrowMouseDown(int dimension)
        {
            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;
            if (SelectionSystem.IsMouseOverVisibleUIElement()) return;
            dragActive = true;

            if (_rigidbodyOfSelected != null)
            {
                _rigidbodyWasKinematicAtDragStart = _rigidbodyOfSelected.isKinematic;
                _rigidbodyOfSelected.isKinematic = true;
            }

            _objectPositionAtDragStart = selected.transform.position;
            _offsetAtDragStart = _objectPositionAtDragStart - ClosestPointOnMovementAxisToMouse(dimension);

            // Draw movement line
            if (_lineRenderer != null)
            {
                var box = SimulationBox.Instance.Bounds;
                Vector3 lineStart = _objectPositionAtDragStart;
                Vector3 lineEnd = _objectPositionAtDragStart;
                lineStart[dimension] = box.min[dimension];
                lineEnd[dimension] = box.max[dimension];

                _lineRenderer.enabled = true;
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPositions(new Vector3[] { lineStart, lineEnd });
            }
        }

        public void OnArrowMouseDrag(int dimension)
        {
            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null || !dragActive) return;

            // Calculate new position based on Mouse-Pos
            var newPos = ClosestPointOnMovementAxisToMouse(dimension) + _offsetAtDragStart;
            var box = SimulationBox.Instance.Bounds;
            float r = selected.boundingRadius;
            newPos.x = Mathf.Clamp(newPos.x, box.min.x + r, box.max.x - r);
            newPos.y = Mathf.Clamp(newPos.y, box.min.y + r, box.max.y - r);
            newPos.z = Mathf.Clamp(newPos.z, box.min.z + r, box.max.z - r);

            // Set new position, see comment in DraggableObject.cs
            selected.transform.position = newPos;
            if (_rigidbodyOfSelected != null)
            {
                _rigidbodyOfSelected.position = selected.transform.position;
            }

            // Update arrow position
            UpdateArrowsDependingOnSelection();

            selected.OnMovedWithGizmo.Invoke(selected);
        }

        public void OnArrowMouseUp(int dimension)
        {
            if (!dragActive) return;
            dragActive = false;

            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
            }

            if (_rigidbodyOfSelected != null)
            {
                _rigidbodyOfSelected.isKinematic = _rigidbodyWasKinematicAtDragStart;
            }
        }
    }
}
