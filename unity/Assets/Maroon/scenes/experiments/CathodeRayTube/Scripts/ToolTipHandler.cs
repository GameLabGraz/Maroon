using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipHandler : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject toolTipPanel;
    [SerializeField] private GameObject toolTipText;

    private void Start()
    {
        toolTipPanel.SetActive(false);
        toolTipText.GetComponent<TextMeshProUGUI>().text = "";
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        toolTipPanel.SetActive(false);
        toolTipText.GetComponent<TextMeshProUGUI>().text = "";
        
        if (Physics.Raycast(ray, out hit) && hit.transform.IsChildOf(transform))
        {
            toolTipPanel.SetActive(true);
            toolTipText.GetComponent<TextMeshProUGUI>().text = hit.transform.name;
        }
    }
}
