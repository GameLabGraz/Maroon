using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SelectionSystem : MonoBehaviour
    {
        private SelectableObject selectedObject = null;
        public UnityEngine.Events.UnityEvent<SelectableObject> OnSelectionChanged;

        // UI references
        [SerializeField] private GUILabelLogic emptySelectionLabel = null;
        [SerializeField] private GameObject uiSelectionParentPanel = null;
        private GameObject lastInstancedSelectedObjectPanel = null;

        // Drag data
        private bool dragActive = false;
        private int dragDimension = -1; // if movementGizmo drag, then this is the drag dimension
        public MovementGizmoController movementGizmo = null;
        [SerializeField] private LineRenderer _lineRenderer;
        private Rigidbody _rigidbodyOfSelected = null;
        private Vector3 _objectPositionAtDragStart;
        private Vector3 _offsetAtDragStart;
        private bool _rigidbodyWasKinematicAtDragStart;



        // Note(MartinR): set newSelectedObject parameter to null to remove current selection
        //  This is a static method so we can handle the case where no selectionSystem instance exists
        public static void SetSelectedObject(SelectableObject newSelectedObject)
        {
            var system = Instance;
            if (system == null) return;

            if (system._lineRenderer != null)
            {
                system._lineRenderer.enabled = false;
            }

            if (system.selectedObject == newSelectedObject) return;
            var prevSelectedObject = system.selectedObject;
            system.selectedObject = newSelectedObject;

            // Invoke callbacks
            prevSelectedObject?.OnObjectSelectedOrDeselected.Invoke(false);
            system.selectedObject?.OnObjectSelectedOrDeselected.Invoke(true);
            system.OnSelectionChanged.Invoke(newSelectedObject);

            // Remove previous UI instanciation
            if (system.lastInstancedSelectedObjectPanel != null)
            {
                GameObject.Destroy(system.lastInstancedSelectedObjectPanel);
            }

            // Create selection UI for newly selected object
            if (system.uiSelectionParentPanel != null && system.selectedObject != null && system.selectedObject.uiSelectionPanelPrefab != null)
            {
                system.lastInstancedSelectedObjectPanel = GameObject.Instantiate(
                    system.selectedObject.uiSelectionPanelPrefab, system.uiSelectionParentPanel.transform);
            }

            // Update empty selection label
            if (system.emptySelectionLabel.gameObject != null)
            {
                system.emptySelectionLabel.gameObject.SetActive(system.selectedObject == null);
            }
        }

        public SelectableObject GetSelectedObject() {
            return selectedObject;
        }

        public static bool IsMouseOverVisibleUIElement()
        {
            // I also return true if mouse is not over window
            Vector2 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            bool isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
            if (isOutside) return true;

            // Note(MartinR): There may be a better way to do this, but for now we raycast the UI to check if we hit anything
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            var eventData = new UnityEngine.EventSystems.PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;
            var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            eventSystem.RaycastAll(eventData, results);

            bool hitVisibleUIElement = false;
            foreach (var hit in results)
            {
                if (!hit.gameObject.activeInHierarchy) { continue; }

                // Note: This check with image and mask is definitly not perfect, as UI elements may use other compontents? to
                //      render on the screen, but it works in pretty much all cases for the current Maroon UI
                var imageComponent = hit.gameObject.GetComponent<UnityEngine.UI.Image>();
                var hasMask = hit.gameObject.GetComponent<UnityEngine.UI.Mask>() != null;
                if (imageComponent != null && imageComponent.IsActive() && !hasMask)
                {
                    hitVisibleUIElement = true;
                    break;
                }
            }

            return hitVisibleUIElement;
        }

        // Update checks if selection changed and handles drag-and-drop logic
        public void Update()
        {
            // Active-Drag-Logic starts here
            if (selectedObject == null)
            {
                dragActive = false;
            }

            if (dragActive)
            {
                if (Input.GetMouseButton(0))
                {
                    // Continue drag
                    Vector3 projectedMousePos = 
                        dragDimension == -1 ? 
                        CameraController.GetMousePointOnPlaneParallelToCamera(_objectPositionAtDragStart) :
                        ClosestPointOnMovementAxisToMouse(dragDimension);

                    var newPos = projectedMousePos + _offsetAtDragStart;
                    if (dragDimension != -1)
                    {
                        var box = SimulationBox.Instance.Bounds;
                        float r = selectedObject.boundingRadius;
                        newPos[dragDimension] = Mathf.Clamp(newPos[dragDimension], box.min[dragDimension] + r, box.max[dragDimension] - r);
                    }

                    // Note(MartinR): Just setting transform.position causes problems when Physics interpolation is enabled.
                    //      _rigidBody.MovePosition also does not seem to do the trick, I guess because it is expected to be called during FixedUpdate?
                    //      Setting rigidBody.position seems to work in all cases
                    selectedObject.transform.position = newPos;
                    if (_rigidbodyOfSelected != null)
                    {
                        _rigidbodyOfSelected.position = selectedObject.transform.position;
                    }

                    // Update arrow position
                    movementGizmo.UpdateArrowsDependingOnSelection();
                    selectedObject.OnMoved.Invoke(selectedObject);
                }
                else
                {
                    // Release drag
                    dragActive = false;
                    if (_lineRenderer != null)
                    {
                        _lineRenderer.enabled = false;
                    }
                    if (_rigidbodyOfSelected != null)
                    {
                        _rigidbodyOfSelected.isKinematic = _rigidbodyWasKinematicAtDragStart;
                    }
                    if (!SimulationBox.Instance.Bounds.Contains(selectedObject.transform.position))
                    {
                        selectedObject.OnDraggedOutOfBounds.Invoke(selectedObject);
                    }
                }
            }



            // Selection and Drag-start logic starts here
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsMouseOverVisibleUIElement()) return;

            // Raycast mouse ray
            float tCurrentlySelected         = 10000.0f;
            float tClosestSelectable         = 10001.0f;
            float tClosestMovementGizmoArrow = 10002.0f;
            MovementGizmoArrow closestGizmoArrow = null;
            SelectableObject   closestSelectable = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = UnityEngine.Physics.RaycastAll(ray);
            foreach (var hit in raycastHits)
            {
                var hitObject = hit.collider.gameObject;
                var selectable = hitObject.GetComponent<SelectableObject>();
                if (selectable != null)
                {
                    if (selectable == selectedObject)
                    {
                        tCurrentlySelected = hit.distance;
                    }
                    if (hit.distance < tClosestSelectable)
                    {
                        tClosestSelectable = hit.distance;
                        closestSelectable = selectable;
                    }
                }

                var gizmoArrow = hitObject.GetComponent<MovementGizmoArrow>();
                if (gizmoArrow != null && gizmoArrow.validDragTarget && hit.distance < tClosestMovementGizmoArrow)
                {
                    tClosestMovementGizmoArrow = hit.distance;
                    closestGizmoArrow = gizmoArrow;
                }
            }

            // Prioritize movement gizmos over new selection (Mostly usefull for transparent plane)
            bool startDrag = false;
            if (closestGizmoArrow != null && tClosestMovementGizmoArrow < tCurrentlySelected)
            {
                startDrag = true;
                dragDimension = closestGizmoArrow.dimension;
            }
            else if (closestSelectable != null)
            {
                SetSelectedObject(closestSelectable);
                startDrag = true;
                dragDimension = -1;
            }
            else
            {
                SetSelectedObject(null);
            }

            // Drag start logic if clicked on an object or movement gizmo
            if (startDrag)
            {
                dragActive = true;
                _objectPositionAtDragStart = selectedObject.transform.position;
                Vector3 projectedMousePos = 
                    dragDimension == -1 ? 
                    CameraController.GetMousePointOnPlaneParallelToCamera(_objectPositionAtDragStart) :
                    ClosestPointOnMovementAxisToMouse(dragDimension);
                _offsetAtDragStart = _objectPositionAtDragStart - projectedMousePos;

                _rigidbodyOfSelected = selectedObject.GetComponent<Rigidbody>();
                if (_rigidbodyOfSelected != null)
                {
                    _rigidbodyWasKinematicAtDragStart = _rigidbodyOfSelected.isKinematic;
                    _rigidbodyOfSelected.isKinematic = true;
                }
                if (_lineRenderer != null && dragDimension != -1)
                {
                    var box = SimulationBox.Instance.Bounds;
                    Vector3 lineStart = _objectPositionAtDragStart;
                    Vector3 lineEnd = _objectPositionAtDragStart;
                    lineStart[dragDimension] = box.min[dragDimension];
                    lineEnd[dragDimension] = box.max[dragDimension];

                    _lineRenderer.enabled = true;
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.SetPositions(new Vector3[] { lineStart, lineEnd });
                }
            }
        }

        // Drag-and-Drop Helpers
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



        // Singleton pattern, see SimulationBox.cs for more comments about the implementation
        private static SelectionSystem _instance;
        public static SelectionSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SelectionSystem>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            // To avoid duplication, this script deletes the attached gameObject if an instance of the Singleton already exists
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
        }
    }
}
