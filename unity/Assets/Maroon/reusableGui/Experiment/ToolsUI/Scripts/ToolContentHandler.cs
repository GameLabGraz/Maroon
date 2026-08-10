using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.UI
{
    public class ToolContentHandler : MonoBehaviour
    {
        private List<GameObject> toolObjects = new List<GameObject>();

        [SerializeField]
        [Tooltip("UI elements that should be hidden when the tool UI is displayed.")]
        private List<GameObject> hideUiObjects = new List<GameObject>();

        private void Start()
        {
            foreach(Transform child in gameObject.transform)
                toolObjects.Add(child.gameObject);
        }

        public void ShowTool(GameObject activeTool)
        {
            // Hide all other tools
            foreach (var tool in toolObjects.Where(tool => tool != activeTool))
                tool.SetActive(false);

            activeTool.SetActive(!activeTool.activeSelf);

            foreach (var uiObject in hideUiObjects)
                uiObject.SetActive(!activeTool.activeSelf);
        }
    }
}
