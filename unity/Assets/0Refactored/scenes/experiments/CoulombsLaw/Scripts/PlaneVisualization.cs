using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

[Serializable]
public class ChargesCallback : SerializableCallback<List<Vector4>> {}
[Serializable]
public class BoolConditionCallback : SerializableCallback<bool> {}
[Serializable]
public class IntConditionCallback : SerializableCallback<int> {}

public class PlaneVisualization : MonoBehaviour, IResetObject, IResetWholeObject
{
    [Header("Possible Visualizations")] 
    public bool voltageVisualizationEnabled = true;
    public bool equipotentialLineVisualizationEnabled = true;
    public bool ironFillingVisualizationEnabled = true;

    [Header("General Settings")] 
    public bool updateDuringRuntime = false;
    public QuantityBool allowInteractions = true;
    
    [Header("Input Parameter")] 
    [Tooltip("Contains all the planes where the visualization should be done. If empty, the gameObject itself is assumed to be the plane.")]
    public List<GameObject> planes;
    [Tooltip("If null, then the planes will be hidden when disabled.")]
    public Material defaultMaterial;
    public Material voltageMaterial;
    public Material equipotentialMaterial;
    
    [Tooltip("This method should return List<Vector4>, where the x,y,z contain the coord. in world position and w contains the charge.")]
    public ChargesCallback onGetChargePosition;
    public IntConditionCallback onGetMaxChargesCount;
    public BoolConditionCallback onAllowVisualization;

    [Header("Assessment System")] 
    public QuantityBool voltageVisualization = false;
    public QuantityBool equipotentialLineVisualization = false;
    public QuantityBool ironFilingVisualization = false;
    
    private int _maxCnt;
    
    private enum Mode
    {
        IronFilling,
        VoltageVis,
        EquipotentialVis,
        Disabled
    }
    
    private Mode _currentMode = Mode.Disabled;
    
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Assert(onGetMaxChargesCount != null);
        _maxCnt = onGetMaxChargesCount.Invoke();
        
        if(planes.Count == 0)
            planes.Add(gameObject);
        
        Disable();
        allowInteractions.onValueChanged.AddListener((value) =>
        {
            if(!value) Disable();
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (_currentMode == Mode.Disabled) return;
        if (!updateDuringRuntime && SimulationController.Instance.SimulationRunning)
            Disable();
        else if(SimulationController.Instance.SimulationRunning && _currentMode == Mode.IronFilling)
            Disable();
        else if (updateDuringRuntime && (_currentMode == Mode.VoltageVis || _currentMode == Mode.EquipotentialVis))
        {
            var vecArray = onGetChargePosition.Invoke();
            var entryCnt = vecArray.Count;
            for (var i = entryCnt; i < 20; ++i)
            {
                // since we cannot resize the vector array -> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
                vecArray.Add(new Vector4(0f, 0f, 0f, 0f));
            }

            foreach (var planeObj in planes)
            {
                var meshRenderer = planeObj.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCnt"), entryCnt);
                meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_Entries"), vecArray);
            }
        }
    }

    public void UpdateInteractions()
    {
        allowInteractions.onValueChanged.Invoke(allowInteractions.Value);
    }

    private void Disable()
    {
        if (defaultMaterial != null)
        {
            foreach (var plane in planes)
            {
                plane.GetComponent<MeshRenderer>().enabled = true;
                plane.GetComponent<MeshRenderer>().material = defaultMaterial;
                var filling = plane.GetComponent<scrIronFilings>();
                if(filling) filling.hideFieldImage();
                plane.SetActive(true);
            }
        }
        else
        {
            foreach (var plane in planes)
            {
                var filling = plane.GetComponent<scrIronFilings>();
                if(filling) filling.hideFieldImage();
                plane.SetActive(false);
            }
        }
        
        _currentMode = Mode.Disabled;

        voltageVisualization.Value = equipotentialLineVisualization.Value = ironFilingVisualization.Value = false;
    }

    public void ShowIronFilling()
    {
        if (onAllowVisualization != null && !onAllowVisualization.Invoke()) return;
        if (!ironFillingVisualizationEnabled) return;

        if (_currentMode == Mode.IronFilling)
        {
            Disable();
            return;
        }
        Disable();
        foreach (var plane in planes)
        {
            plane.SetActive(true);
            plane.GetComponent<MeshRenderer>().enabled = false;
            var filling = plane.GetComponent<scrIronFilings>();
            Debug.Assert(filling != null);
            filling.generateFieldImage();
        }
        
        _currentMode = Mode.IronFilling;
        ironFilingVisualization.Value = true;
    }

    public void ShowVoltageVisualization()
    {
        if (onAllowVisualization != null && !onAllowVisualization.Invoke()) return;
        if (!voltageVisualizationEnabled) return;

        if (_currentMode == Mode.VoltageVis)
        {
            Disable();
            return;
        }
        Disable();
        ShowShader(voltageMaterial);

        _currentMode = Mode.VoltageVis;
        voltageVisualization.Value = true;
    }
    
    public void StopVisualization()
    {
        Disable();
    }
    
    public void ShowEquipotentialLineVisualization()
    {    
        if (onAllowVisualization != null && !onAllowVisualization.Invoke()) return;
        if (!equipotentialLineVisualizationEnabled) return;

        if (_currentMode == Mode.EquipotentialVis)
        {
            Disable();
            return;
        }

        Disable();
        ShowShader(equipotentialMaterial);
        _currentMode = Mode.EquipotentialVis;
        equipotentialLineVisualization.Value = true;
    }

    private void ShowShader(Material mat)
    {
        SimulationController.Instance.SimulationRunning = false;
        var vecArray = onGetChargePosition.Invoke();
        var entryCnt = vecArray.Count;
        for (var i = entryCnt; i < _maxCnt; ++i)
        {
            // since we cannot resize the vector array -> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
            vecArray.Add(new Vector4(0f, 0f, 0f, 0f));
        }
        
        foreach (var planeObj in planes)
        {
            planeObj.SetActive(true);
            var meshRenderer = planeObj.GetComponent<MeshRenderer>();
            Debug.Assert(meshRenderer);
            meshRenderer.enabled = true;
            meshRenderer.material = mat;
            meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCnt"), entryCnt);
            meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_Entries"), vecArray);
        }
    }

    public void ResetObject()
    {
        // Disable();
    }

    public void ResetWholeObject()
    {
        Disable();
    }

    public IQuantity GetVoltageVis()
    {
        return voltageVisualization;
    }

    public IQuantity GetEquipotentialLineVis()
    {
        return equipotentialLineVisualization;
    }

    public IQuantity GetIronFilingVis()
    {
        return ironFilingVisualization;
    }
    
    public void ForceVoltageVisualization(bool show)
    {
        if(_currentMode == Mode.VoltageVis)
            Disable();
        if(show)
            ShowVoltageVisualization();
    }
    
    public void ForceEquipotentialLineVisualization(bool show)
    {
        if(_currentMode == Mode.EquipotentialVis)
            Disable();
        if(show)
            ShowEquipotentialLineVisualization();
    }
    
    public void ForceIronFilingVisualization(bool show)
    {
        if(_currentMode == Mode.IronFilling)
            Disable();
        if(show)
            ShowIronFilling();
    }
}
