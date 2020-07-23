using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SelectV3Event : UnityEvent<Vector3> {}

public class PC_SelectScript : MonoBehaviour
{
    public enum SelectType
    {
        ChargeSelect,
        VisualizationPlaneSelect,
        VoltmeterSelect,
        CubeSelect,
        WhiteboardSelect,
        RulerSelect
    }
    
    // Selected Object
    public List<GameObject> highlightObjects = new List<GameObject>();
    public SelectType type;
    public string nameKey;

    [Header("Events")] 
    public SelectV3Event onPositionChanged;
    public SelectV3Event onRotationChanged;

    private CoulombLogic _coulombLogic;
    private void OnDisable()
    {
        if (!_coulombLogic || _coulombLogic.GetComponent<PC_SelectionHandler>().selectedObject != this) return;
        Deselect();
        DeselectMe();
    }

    private void OnMouseDown()
    {
        // Debug.Log("Mouse down on Select: " + gameObject.name);

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
        // Debug.Log("Select");

        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj)
                _coulombLogic = obj.GetComponent<CoulombLogic>();
        }
        var selectHandler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        selectHandler.SelectObject(this);
        
        foreach (var highlightObj in highlightObjects)
        {
            var mats = highlightObj.GetComponent<MeshRenderer>().sharedMaterials.ToList();
            if(!mats.Contains(selectHandler.highlightMaterial))
                mats.Add(selectHandler.highlightMaterial);
            highlightObj.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        }
    }
    
    public void Deselect()
    {
        // Debug.Log("Deselect");
        var selectHandler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        foreach (var highlightObj in highlightObjects)
        {
            var mats = highlightObj.GetComponent<MeshRenderer>().sharedMaterials.ToList();
            mats.Remove(selectHandler.highlightMaterial);
            highlightObj.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        }
    }

    public void DeselectMe()
    {
        if (!_coulombLogic) return;
        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>(); 
        handler.SelectObject(null);
    }
    
    public void PositionChanged()
    {
        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj)
                _coulombLogic = obj.GetComponent<CoulombLogic>();
        }

        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.PositionChanged();
    }
    
    public void RotationChanged()
    {
        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj)
                _coulombLogic = obj.GetComponent<CoulombLogic>();
        }

        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.RotationChanged();
    }

    private void OnDestroy()
    {
        if(!_coulombLogic) return;
        var handler = _coulombLogic.GetComponent<PC_SelectionHandler>();
        if (handler && handler.selectedObject == this)
            handler.SelectObject(null);
    }
}
