using System.Collections.Generic;
using UnityEngine;

namespace Assets.Maroon.reusableGui.Experiment.ToolsUI.Scripts
{
    public class ToolDictionary : MonoBehaviour
    {
        private List<GameObject> childGameObjects = new List<GameObject>();
           
        private void Start()
        {
            foreach(Transform child in gameObject.transform)
            { 
                childGameObjects.Add(child.gameObject);
            }
        }

        public void HideAllOtherTools(GameObject toolToRemainActive)
        { 
            foreach(var tool in childGameObjects)
            {
                if(toolToRemainActive != tool)
                    tool.SetActive(false);
            }

            toolToRemainActive.SetActive(true);
        }
    }
}
