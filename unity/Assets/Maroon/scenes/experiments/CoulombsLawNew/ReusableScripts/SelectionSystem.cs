using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SelectionSystem : MonoBehaviour
    {
        private SelectableObject selectedObject = null;
        public UnityEngine.Events.UnityEvent<SelectableObject> OnSelectionChanged;

        // Note(MartinR): gameObject may be null to remove current selection
        public void SetSelectedObject(SelectableObject newSelectedObject)
        {
            if (selectedObject == newSelectedObject) return;
            var prevSelectedObject = selectedObject;
            selectedObject = newSelectedObject;

            // Invoke callbacks
            prevSelectedObject?.OnObjectSelectedOrDeselected.Invoke(false);
            selectedObject?.OnObjectSelectedOrDeselected.Invoke(true);
            OnSelectionChanged.Invoke(newSelectedObject);
        }

        public SelectableObject GetSelectedObject() {
            return selectedObject;
        }

        public void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

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

            // Don't deselect if we clicked somewhere in UI
            {
                // Note(MartinR): There may be a better way to do this, but for now we raycast the UI to check if we hit anything
                var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                var eventData = new UnityEngine.EventSystems.PointerEventData(eventSystem);
                eventData.position = Input.mousePosition;
                var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                eventSystem.RaycastAll(eventData, results);
                if (results.Count > 0)
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
                    if (_instance == null) _instance = new GameObject("SelectionSystem").AddComponent<SelectionSystem>();
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
