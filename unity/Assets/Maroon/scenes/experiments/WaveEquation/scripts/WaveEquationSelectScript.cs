using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveEquationSelectScript : MonoBehaviour
{
    public enum SelectObjectType
    {
        SourceSelect,
        VisualizationPlaneSelect,
    }
    
    public List<GameObject> highlightObjects = new List<GameObject>();
    public SelectObjectType type;
    public string nameKey;
    
    private WaveEquationPoolHandler _waveLogic;
    private void OnDisable()
    {
        if (!_waveLogic || _waveLogic.GetComponent<WaveEquation_SelectionHandler>().selectedObject != this) return;
        Deselect();
        DeselectMe();
    }

    private void OnMouseDown()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<WaveEquationPoolHandler>();
        }

        Select();
    }

    public void Select()
    {        
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<WaveEquationPoolHandler>();
        }
        var selectHandler = _waveLogic.GetComponent<WaveEquation_SelectionHandler>();
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
        var selectHandler = _waveLogic.GetComponent<WaveEquation_SelectionHandler>();
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
        var handler = _waveLogic.GetComponent<WaveEquation_SelectionHandler>(); 
        handler.SelectObject(null);
    }
    
    public void PositionChanged()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<WaveEquationPoolHandler>();
        }

        var handler = _waveLogic.GetComponent<WaveEquation_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.PositionChanged();
    }
    
    private void OnDestroy()
    {
        if(!_waveLogic) return;
        var handler = _waveLogic.GetComponent<WaveEquation_SelectionHandler>();
        if (handler && handler.selectedObject == this)
            handler.SelectObject(null);
    }
}
