using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIElement : MonoBehaviour
{
    [SerializeField]
    public GameObject UiElement;
    private bool isactive = true;


    public void toggleElement()
    {
        isactive = !isactive;
        UiElement.active = isactive;
    }
}
