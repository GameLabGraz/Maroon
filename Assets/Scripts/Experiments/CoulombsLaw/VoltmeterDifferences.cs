using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VoltmeterDifferences : MonoBehaviour
{
    public enum DifferenceMode
    {
        Distance,
        VoltageDifference,
        PotentialDifference
    }
    
    [NotNull] public Voltmeter Voltmeter1;
    [NotNull] public Voltmeter Voltmeter2;
    [Tooltip("Resistor is only needed when using the Potential Difference Mode")]
    public Resistor resistor;
    public DifferenceMode Mode;
    
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
        if (!Voltmeter1.isActiveAndEnabled || !Voltmeter2.isActiveAndEnabled)
            _textMeshProUgui.text = "--- " + GetCurrentUnit();
        else
        {
            _textMeshProUgui.text = GetDifference() + " " + GetCurrentUnit();
        }
    }
    
    private string GetDifference(){
        switch (Mode)
        {
            case DifferenceMode.Distance:
                var plainDiff = Vector3.Distance(Voltmeter1.transform.position, Voltmeter2.transform.position);
                _currentValue = _coulombLogic.WorldToCalcSpace(plainDiff);
                return (_currentValue * 100f).ToString("F");
            
            case DifferenceMode.VoltageDifference:
                _currentValue = Mathf.Abs(Voltmeter1.GetPotentialInMicroVolt() - Voltmeter2.GetPotentialInMicroVolt());
                return GetCurrentFormattedString();
            
            case DifferenceMode.PotentialDifference:
                _currentValue = Mathf.Abs(Voltmeter1.GetPotentialInMicroVolt() - Voltmeter2.GetPotentialInMicroVolt()) / resistor.resistance;
                return GetCurrentFormattedString();
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        return "Not Implemented";
    }

    private string GetCurrentFormattedString()
    {
        var check = _currentValue;
        for (var cnt = 0; check < 1f && cnt < 2; ++cnt)
        {
            check *= Mathf.Pow(10, 3);
        }
            
        Debug.Log("START: " + _currentValue.ToString("F") + " - END: "+ check.ToString("F"));
        return check.ToString("F");   
    }

    private string GetCurrentUnit()
    {
        var unit = "";
        
        switch (Mode)
        {
            case DifferenceMode.Distance: return "cm"; // no other unit will be used (at least not yet -> maybe we will use m in VR)
            case DifferenceMode.VoltageDifference: unit = "V";
                break;
            case DifferenceMode.PotentialDifference: unit = "A";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var check = _currentValue;
        if (check > 1f)
            return unit;
        check *= Mathf.Pow(10, 3);
        if (check > 1f)
            return "m" + unit;
        return "\u00B5" + unit;
    }
}
