using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_SelectScript : MonoBehaviour
{
    public enum SelectType
    {
        ChargeSelect
    }
    
    // Selected Object
    //TODO: highlight the object
    public GameObject highlightObject;
    public SelectType type;
    
    private CoulombLogic _coulombLogic;

    private void OnMouseDown()
    {
        Debug.Log("Mouse down on Select: " + gameObject.name);
        
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();
        
        Select();
    }

    public void Select()
    {
        switch (type)
        {
            case SelectType.ChargeSelect:
                _coulombLogic.GetComponent<PC_ChargeSelect>().SelectObject(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Deselect()
    {
        
    }
}
