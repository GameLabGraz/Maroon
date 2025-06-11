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
            bool raycastHitSelectable = false;
            if (UnityEngine.Physics.Raycast(ray, out hitInfo))
            {
                var hitObject = hitInfo.collider.gameObject;
                var selectableObject = hitObject.GetComponent<SelectableObject>();
                if (selectableObject != null)
                {
                    raycastHitSelectable = true;
                    SetSelectedObject(selectableObject);
                }
            }

            // Find out if we clicked on UI (Don't deselect if we clicked on UI)
            bool clickedOnUIElement = false;
            {
                // Note(MartinR): There may be a better way to do this, but for now we raycast the UI to check if we hit anything
                var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                var eventData = new UnityEngine.EventSystems.PointerEventData(eventSystem);
                eventData.position = Input.mousePosition;
                var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                eventSystem.RaycastAll(eventData, results);
                if (results.Count > 0)
                {
                    clickedOnUIElement = true;
                }
            }

            // Deselect current particle if we clicked somewhere that wasn't UI (e.g. empty space/background)
            if (!raycastHitSelectable && !clickedOnUIElement)
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
                    // Check if scene already contains a singleton instance
                    _instance = GameObject.FindObjectOfType<SelectionSystem>();
                    // Create instance if none exists in scene
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
                Debug.LogWarning("SelectionSystem instance was destroyed due to duplication");
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
