using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipHandler : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject partInfoToggle;
    private bool collision;
    private Transform hitObject;
    GUIStyle guiStyle = new GUIStyle();

    private void Start()
    {
        guiStyle.normal.textColor = Color.black;
        guiStyle.fontSize = 14;
    }

    private void Update()
    {
        if (!partInfoToggle.GetComponent<Toggle>().isOn || Camera.main != camera)
            return;

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        collision = false;
        if (Physics.Raycast(ray, out hit, 10.0f, LayerMask.NameToLayer("IgnorePostProcessing")) && hit.transform.CompareTag("crtPart"))
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
            
            GUI.Label(new Rect(convertedGUIPos.x + 15, convertedGUIPos.y, 200, 20), LanguageManager.Instance.GetString(hitObject.name), guiStyle);
        }
    }
}
