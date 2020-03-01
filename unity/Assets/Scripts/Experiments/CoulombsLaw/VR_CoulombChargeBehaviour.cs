using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_CoulombChargeBehaviour : MonoBehaviour, IResetWholeObject
{
    public VRTK_SnapDropZone CorrespondingSnapDropZone;
    
    [Header("Color Settings")] 
    public Color negativeColorMin = new Color(0.267f, 0.514f, 1f);
    public Color negativeColorMax = new Color(0f, 0f, 1f);
    public Color positiveColorMin = new Color(1f, 0.524f, 0.524f);
    public Color positiveColorMax = new Color(1f, 0f, 0f);
    public Color neutralColor = new Color(0f, 0.745f, 0.127f);


    private bool _hasUpdatedCharge = true;
    private float _updateValue = 1f;
    private bool _hasUpdatedFixed = true;
    private bool _fixedPos = false;
    private bool _checkOldSnapped = false;
    private GameObject _oldSnapped;

    private List<GameObject> _resetObjList = new List<GameObject>();
    
    private void Start()
    {
        if (CorrespondingSnapDropZone == null)
            CorrespondingSnapDropZone = GetComponent<VRTK_SnapDropZone>();

        _updateValue = 0.0f;
        _fixedPos = false;
        _hasUpdatedCharge = _hasUpdatedFixed = true;
    }

    private void Update()
    {
        if(!_hasUpdatedCharge)
            UpdateCharge();
        if (!_hasUpdatedFixed)
            UpdateFixedPosition();
    }


    public void SetChargeValue(object o, Control3DEventArgs e)
    {
        _hasUpdatedCharge = false;
        _updateValue = e.value;
       
        UpdateCharge();
    }

    private void UpdateCharge()
    {
        var snapped = CorrespondingSnapDropZone.GetCurrentSnappedObject();
        if (_checkOldSnapped && _oldSnapped == snapped) return;
        _oldSnapped = snapped;
        _checkOldSnapped = false;

        if(!snapped) return;
        var behaviour = snapped.GetComponent<CoulombChargeBehaviour>();
        if (!behaviour) return;
        
        if(Mathf.Abs(_updateValue) < 0.001f)
            behaviour.SetCharge(0f, neutralColor);
        else if(_updateValue < 0f)
            behaviour.SetCharge(_updateValue, negativeColorMin + (negativeColorMax - negativeColorMin) * Mathf.Abs(_updateValue / 5f));
        else
            behaviour.SetCharge(_updateValue, positiveColorMin + (positiveColorMax - positiveColorMin) * Mathf.Abs(_updateValue / 5f));
        _hasUpdatedCharge = true;
    }


    private void UpdateFixedPosition()
    {
        var snapped = CorrespondingSnapDropZone.GetCurrentSnappedObject();
        if (_checkOldSnapped && _oldSnapped == snapped) return;
        _oldSnapped = snapped;
        _checkOldSnapped = false;
        if(!snapped) return;
        var behaviour = snapped.GetComponent<CoulombChargeBehaviour>();
        if (!behaviour) return;
        behaviour.SetFixedPosition(_fixedPos);
        _hasUpdatedFixed = true;
    }

    public void UpdateValues()
    {
        Debug.Log("Update Values: " + _updateValue + " - " + _fixedPos);
        _hasUpdatedCharge = false;
        _hasUpdatedFixed = false;
        _checkOldSnapped = true;

        var obj = _oldSnapped;
        if(obj)
        {
            obj.GetComponent<CoulombChargeBehaviour>().SetInUse(false);
            Debug.Log("New Object: " + obj.name);

            _resetObjList.Add(obj);
        }
        
        UpdateCharge();
        UpdateFixedPosition();
    }

    public void SetFixedPosition(bool isFixed)
    {
        _hasUpdatedFixed = false;
        _fixedPos = isFixed;
        UpdateFixedPosition();
    }

    public void ResetObject()
    {
        //do nothing
    }

    public void ResetWholeObject()
    {
        foreach (var charge in _resetObjList)
        {
            if(!charge) continue;
            
            var chargeBeh = charge.GetComponent<CoulombChargeBehaviour>();
            if (chargeBeh && !chargeBeh.IsInUse())
            {
                Destroy(charge);
            }
        }
    }
}
