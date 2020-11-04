using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointWaveSelectScript : MonoBehaviour
{
    public enum SelectObjectType
    {
        SourceSelect,
        VisualizationPlaneSelect,
    }
    
    // Selected Object
    public List<GameObject> highlightObjects = new List<GameObject>();
    public SelectObjectType type;
    public string nameKey;
    
    private PointWavePoolHandler _waveLogic;
    private void OnDisable()
    {
        if (!_waveLogic || _waveLogic.GetComponent<PointWave_SelectionHandler>().selectedObject != this) return;
        Deselect();
        DeselectMe();
    }

    private void OnMouseDown()
    {
        //ss Debug.Log("Mouse down on Select: " + gameObject.name);

        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }

        Select();
    }

    public void Select()
    {        
        Debug.Log("Select");

        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }
        var selectHandler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
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
         Debug.Log("Deselect");
        var selectHandler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        foreach (var highlightObj in highlightObjects)
        {
            var mats = highlightObj.GetComponent<MeshRenderer>().sharedMaterials.ToList();
            mats.Remove(selectHandler.highlightMaterial);
            highlightObj.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        }
    }

    public void DeselectMe()
    {
        if (!_waveLogic) return;
        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>(); 
        handler.SelectObject(null);
    }
    
    public void PositionChanged()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }

        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.PositionChanged();
    }
    
   /* public void RotationChanged()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }

        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.RotationChanged();
    }*/

    private void OnDestroy()
    {
        if(!_waveLogic) return;
        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        if (handler && handler.selectedObject == this)
            handler.SelectObject(null);
    }
}
