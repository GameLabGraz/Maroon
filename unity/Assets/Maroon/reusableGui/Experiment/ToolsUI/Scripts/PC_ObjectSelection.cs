using System.Collections.Generic;
using UnityEngine;

namespace Assets.Maroon.reusableGui.Experiment.ToolsUI.Scripts
{
    public class PC_ObjectSelection : MonoBehaviour
    {
        public enum SelectObjectType
        {
            SourceSelect,
            VisualizationPlaneSelect,
        }

        public List<GameObject> highlightObjects = new List<GameObject>();
        public SelectObjectType type;

        private PC_ObjectSelectionHandler _objectSelectionHandler;

        void Start()
        {
            if (!_objectSelectionHandler)
                _objectSelectionHandler = GameObject.FindObjectOfType<PC_ObjectSelectionHandler>();
        }

        private void OnMouseDown()
        {
            if (!_objectSelectionHandler)
                _objectSelectionHandler = GameObject.FindObjectOfType<PC_ObjectSelectionHandler>();

            Select();
        }

        public void Select()
        {
            if (this.type == SelectObjectType.SourceSelect)
            {
                _objectSelectionHandler.SelectObject(this);
            }
            else
            {
                _objectSelectionHandler.DeselectAll();
            }
        }
    }
}
