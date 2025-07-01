using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SelectionSystem : MonoBehaviour
    {
        private SelectableObject selectedObject = null;

        [SerializeField] private TMPro.TMP_Text emptySelectionLabel = null;
        [SerializeField] private GameObject uiSelectionParentPanel = null;
        private GameObject lastInstancedSelectedObjectPanel = null;
        public MovementGizmoController movementGizmo = null;

        public UnityEngine.Events.UnityEvent<SelectableObject> OnSelectionChanged;

        // Note(MartinR): set newSelectedObject parameter to null to remove current selection
        //  This is a static method so we can handle the case where no selectionSystem instance exists
        public static void SetSelectedObject(SelectableObject newSelectedObject)
        {
            var system = Instance;
            if (system == null) return;

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
            if (system.emptySelectionLabel != null)
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

        // Update checks if user changes selection with mouse-clicks
        public void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsMouseOverVisibleUIElement()) return;

            SelectableObject previousSelection = selectedObject;

            // Check if we clicked on a selectable object, and update selection if we did
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool deselectObject = true;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo))
            {
                // Check if we hit a selectable object
                var hitObject = hitInfo.collider.gameObject;
                var selectableObject = hitObject.GetComponent<SelectableObject>();
                if (selectableObject != null)
                {
                    deselectObject = false;
                    SetSelectedObject(selectableObject);
                }

                // Don't deselect if we click on movement arrow
                if (hitObject.GetComponent<MovementGizmoArrow>() != null)
                {
                    deselectObject = false;
                }
            }

            // Deselect current particle if we clicked somewhere that wasn't UI nor MovementGizmo (e.g. empty space/background)
            if (deselectObject)
            {
                SetSelectedObject(null);
            }
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
