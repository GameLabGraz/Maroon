using GEAR.Gadgets.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Tools
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
