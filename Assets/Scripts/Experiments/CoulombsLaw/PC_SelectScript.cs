using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_SelectScript : MonoBehaviour
{
    // Selected Object
    //TODO: highlight the object
    public GameObject highlightObject;

    public UnityEvent<GameObject> selectedComponent;


    private void OnMouseDown()
    {
        Debug.Log("Mouse down on Select: " + gameObject.name);
        selectedComponent.Invoke(gameObject);
    }
}
