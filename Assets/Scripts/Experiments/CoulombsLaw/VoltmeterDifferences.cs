using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VoltmeterDifferences : MonoBehaviour
{
    public VoltmeterMeasuringPoint positiveMeasuringPoint;
    public VoltmeterMeasuringPoint negativeMeasuringPoint;

    private CoulombLogic _coulombLogic;
    private TextMeshProUGUI _textMeshProUgui;

    private float _currentValue;
    
    // Start is called before the first frame update
    void Start()
    {
        _textMeshProUgui = GetComponent<TextMeshProUGUI>();
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!positiveMeasuringPoint.isActiveAndEnabled || !negativeMeasuringPoint.isActiveAndEnabled)
            _textMeshProUgui.text = "--- " + GetCurrentUnit();
        else
        {
            _textMeshProUgui.text = GetDifference() + " " + GetCurrentUnit();
        }
    }
    
    private string GetDifference(){
        _currentValue = positiveMeasuringPoint.GetPotentialInMicroVolt() - negativeMeasuringPoint.GetPotentialInMicroVolt();
        return GetCurrentFormattedString();
    }

    private string GetCurrentFormattedString()
    {
        var check = _currentValue;
        for (var cnt = 0; Mathf.Abs(check) < 1f && cnt < 2; ++cnt)
        {
            check *= Mathf.Pow(10, 3);
        }
            
//        Debug.Log("START: " + _currentValue.ToString("F") + " - END: "+ check.ToString("F"));
        return check.ToString("F");   
    }

    private string GetCurrentUnit()
    {
        var unit = "V";
        var check = _currentValue;
        if (check > 1f)
            return unit;
        check *= Mathf.Pow(10, 3);
        if (check > 1f)
            return "m" + unit;
        return "\u00B5" + unit;
    }
}
