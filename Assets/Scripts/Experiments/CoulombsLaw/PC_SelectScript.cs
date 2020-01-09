﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_SelectScript : MonoBehaviour
{
    public enum SelectType
    {
        ChargeSelect,
        VisualizationPlaneSelect,
        VoltmeterSelect
    }
    
    // Selected Object
    //TODO: highlight the object
    public GameObject highlightObject;
    public SelectType type;
    public string nameKey;
    
    private CoulombLogic _coulombLogic;

    private void OnMouseDown()
    {
        Debug.Log("Mouse down on Select: " + gameObject.name);

        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj)
                _coulombLogic = obj.GetComponent<CoulombLogic>();
        }

        Select();
    }

    public void Select()
    {
        if(_coulombLogic)
            _coulombLogic.GetComponent<PC_SelectionHandler>().SelectObject(this);
    }
    
    public void Deselect()
    {
        
    }

    public void DeselectMe()
    {
        if (!_coulombLogic) return;
        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>(); 
        handler.SelectObject(null);
    }
    
    public void PositionChanged()
    {
        Debug.Log("Pos Changed 1");
        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj)
                _coulombLogic = obj.GetComponent<CoulombLogic>();
        }
        Debug.Log("Pos Changed 2");

        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.PositionChanged();
    }
    
}
