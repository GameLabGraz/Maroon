using Maroon.Physics;
using TMPro;
using UnityEngine;

public class VoltmeterDifferences : MonoBehaviour
{
    public VoltmeterMeasuringPoint positiveMeasuringPoint;
    public VoltmeterMeasuringPoint negativeMeasuringPoint;

    private CoulombLogic _coulombLogic;
    public TextMeshPro textMeshPro;
    public TextMeshProUGUI textMeshProGUI;
    public TextMeshPro textMeshProUnit;

    public bool onPerDefault = true;
    public bool showUnitInText = true;

    [Header("Assessment System")]
    public QuantityFloat currentValue;

    private bool _isOn;
    
    // Start is called before the first frame update
    void Start()
    {
        if(textMeshPro == null)
            textMeshPro = GetComponent<TextMeshPro>();

        if (textMeshProGUI == null)
            textMeshProGUI = GetComponent<TextMeshProUGUI>();
        
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
            SetText("--- " + (showUnitInText? GetCurrentUnit() : ""));
        else
        {
            SetText(GetDifference() + (showUnitInText? " " + GetCurrentUnit() : ""));
        }

        if (!showUnitInText && textMeshProUnit)
        {
            textMeshProUnit.text = GetCurrentUnit();
        }
    }

    private void SetText(string text)
    {
        if (textMeshPro)
            textMeshPro.text = text;
        if (textMeshProGUI)
            textMeshProGUI.text = text;
    }
    
    private string GetDifference(){
        currentValue = positiveMeasuringPoint.GetPotentialInMicroVolt() - negativeMeasuringPoint.GetPotentialInMicroVolt();
        return GetCurrentFormattedString();
    }

    private string GetCurrentFormattedString()
    {
        float check = currentValue;
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
        var check = currentValue;
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
