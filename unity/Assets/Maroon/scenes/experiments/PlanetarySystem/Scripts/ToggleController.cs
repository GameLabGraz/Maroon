using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Toggle toggle;


    private void Start()
    {
        SetupToggle();
    }

    public void UIToggle(bool isOn)
    {
        //Debug.Log("");

    }

    private void SetupToggle()
    {
        toggle.onValueChanged.AddListener(UIToggle);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(UIToggle);
    }

}
