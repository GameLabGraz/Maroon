using TMPro;
using UnityEngine;

public class VoltmeterDifferences : MonoBehaviour
{
    public VoltmeterMeasuringPoint positiveMeasuringPoint;
    public VoltmeterMeasuringPoint negativeMeasuringPoint;

    private CoulombLogic _coulombLogic;
    public TextMeshPro textMeshPro;
    public TextMeshPro textMeshProUnit;

    public bool onPerDefault = true;
    public bool showUnitInText = true;
    
    private float _currentValue;
    private bool _isOn;
    
    // Start is called before the first frame update
    void Start()
    {
        if(textMeshPro == null)
            textMeshPro = GetComponent<TextMeshPro>();
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);

        _isOn = onPerDefault;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isOn || (!positiveMeasuringPoint.isActiveAndEnabled || !negativeMeasuringPoint.isActiveAndEnabled))
            textMeshPro.text = "--- " + (showUnitInText? GetCurrentUnit() : "");
        else
        {
            textMeshPro.text = GetDifference() + (showUnitInText? " " + GetCurrentUnit() : "");
        }

        if (!showUnitInText && textMeshProUnit)
        {
            textMeshProUnit.text = GetCurrentUnit();
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

    public void TurnOn()
    {
        _isOn = true;
    }

    public void TurnOff()
    {
        _isOn = false;
    }
}
