using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SelectableObject : MonoBehaviour
    {
        public GameObject uiSelectionPanelPrefab = null;
        public bool enableMovementGizmo = false;
        [Tooltip("Display an orange highlight circle behind the object when selected")]
        public bool enableSelectionHighlightCircle = true;
        [Tooltip("Used to determine the size of the selection-marker and movement arrows")]
        public float boundingRadius = 1.0f;

        public UnityEngine.Events.UnityEvent<bool> OnObjectSelectedOrDeselected; // Is called when object was selected/deselected
        public UnityEngine.Events.UnityEvent<SelectableObject> OnMovedWithGizmo;

        private void OnDestroy()
        {
            // Remove selection from this object if the gameObject is destroyed
            var selectionSystem = SelectionSystem.Instance;
            if (selectionSystem.GetSelectedObject() == this)
            {
                selectionSystem.SetSelectedObject(null);
            }
        }
    }
}
