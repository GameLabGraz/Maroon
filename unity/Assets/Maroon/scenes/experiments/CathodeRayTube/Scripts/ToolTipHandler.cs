using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipHandler : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject partInfoToggle;
    private bool collision;
    private Transform hitObject;

    private void Update()
    {
        if (!partInfoToggle.GetComponent<Toggle>().isOn)
            return;
        
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        collision = false;
        if (Physics.Raycast(ray, out hit) && hit.transform.IsChildOf(transform))
        {
            collision = true;
            hitObject = hit.transform;
        }
    }
    
    private void OnGUI()
    {
        if (collision)
        {
            Vector2 screenPos = Event.current.mousePosition;
            Vector2 convertedGUIPos = GUIUtility.ScreenToGUIPoint(screenPos);
 
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(convertedGUIPos.x + 15, convertedGUIPos.y, 200, 20), hitObject.name);
        }
    }
}
