using GEAR.Localization.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Maroon.UI;
using Slider = GameLabGraz.UI.Slider;

public class WaveEquation_SelectionHandler : MonoBehaviour
{
    [Header("Charge Specific Game Objects")]
    public GameObject inputXVariable;
    public GameObject inputZVariable;

    public Slider waveSourceAmplitudeSlider;
    public Slider waveSourceLengthSlider;
    public Slider waveSourceFrequencySlider;
    public Slider waveSourcePhaseSlider;

    public TMP_Text amplitudeInputField;
    public TMP_Text waveLengthInputField;
    public TMP_Text frequencyInputField;
    public TMP_Text phaseInputField;

    public GameObject SourceButtonAddDelete;
    public GameObject SourceButtonText;
   
    public GameObject SourcePrefab;   

    [Header("General Game Objects")] 
    public GameObject xVariablePosition;
    public GameObject zVariablePosition;


    [Header("Others")] public WaveEquationSelectScript selectedObject = null;
    public Material highlightMaterial;

    private WaveEquationPoolHandler _waveLogic;

    private void Start()
    {

        var obj = GameObject.Find("PoolHandler");
        if (obj) _waveLogic = obj.GetComponent<WaveEquationPoolHandler>();

        AdaptButtonTextCharge();

        inputXVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
                CheckVariable(endVal, Vector3.right);
        });

        
        inputZVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect) 
                CheckVariable(endVal, Vector3.forward);
        });
        
        waveSourceAmplitudeSlider.onValueChanged.AddListener((newValue) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
                selectedObject.GetComponent<WaveEquationSource>().WaveAmplitude = newValue;
        });

        waveSourceLengthSlider.onValueChanged.AddListener((newValue) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
                selectedObject.GetComponent<WaveEquationSource>().WaveLength = newValue;
        });

        waveSourceFrequencySlider.onValueChanged.AddListener((newValue) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
                selectedObject.GetComponent<WaveEquationSource>().WaveFrequency = newValue;
        });

        waveSourcePhaseSlider.onValueChanged.AddListener((newValue) =>
        {
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)         
                selectedObject.GetComponent<WaveEquationSource>().WavePhase = newValue;      
        });


        xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckVariable(endVal, Vector3.right);
            _waveLogic.onParameterChange.Invoke();
        });
        zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckVariable(endVal, Vector3.forward);
            _waveLogic.onParameterChange.Invoke();
        });
        

        SourceButtonAddDelete.GetComponent<Button>().onClick.AddListener(() =>
        {           
            if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
            {
                //DELETE
                DeleteSelectedCharge();
            }
            else
            {
                //CREATE 
                _waveLogic.CreateSource(SourcePrefab, GetInputPosition(WaveEquationSelectScript.SelectObjectType.SourceSelect),
                    waveSourceAmplitudeSlider.value , waveSourceLengthSlider.value, waveSourceFrequencySlider.value, waveSourcePhaseSlider.value,  false);               
                AdaptButtonTextCharge();
            }
        });
        
        SetSourceVariableFields();
        AdaptVariableFields();
    }

    private void DeleteSelectedCharge()
    {
        // TODO Removal of Object
        var waveSource = selectedObject.GetComponent<WaveEquationSource>();
        SelectObject(null);
        _waveLogic.RemoveSource(waveSource);
        AdaptButtonTextCharge();
    }

    private void Update()
    {
        if (selectedObject == null) return;

        if (!selectedObject.isActiveAndEnabled)
        {
            SelectObject(null);
            return;
        }
    }


    private void AdaptButtonTextCharge()
    {
        if (SourceButtonText == null || SourceButtonText.GetComponent<LocalizedTMP>() == null) return;
        if (!SourceButtonText) return;
        SourceButtonText.GetComponent<LocalizedTMP>().Key =
            selectedObject == null || selectedObject.type != WaveEquationSelectScript.SelectObjectType.SourceSelect
                ? "Button_Add"
                : "Button_Delete";
    }

    public void SetSourceVariableFields()
    {
        inputXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
        inputXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _waveLogic.xMax2d;
        inputZVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
        inputZVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _waveLogic.yMax2d;


        if (selectedObject != null && selectedObject.type == WaveEquationSelectScript.SelectObjectType.SourceSelect)
        {
            waveSourceAmplitudeSlider.AllowReset(false);
            waveSourceLengthSlider.AllowReset(false);
            waveSourceFrequencySlider.AllowReset(false);
            waveSourcePhaseSlider.AllowReset(false);

            var obj = selectedObject.GetComponent<WaveEquationSource>();

            waveSourceAmplitudeSlider.SetSliderValue(obj.WaveAmplitude);
            waveSourceLengthSlider.SetSliderValue(obj.WaveLength);
            waveSourceFrequencySlider.SetSliderValue(obj.WaveFrequency);
            waveSourcePhaseSlider.SetSliderValue(obj.WavePhase);
           
            amplitudeInputField.text = obj.WaveAmplitude.ToString("F");
            waveLengthInputField.text = obj.WaveLength.ToString("F");
            frequencyInputField.text = obj.WaveFrequency.ToString("F");
            phaseInputField.text = obj.WavePhase.ToString("F");
        }
    }

    public void AdaptVariableFields()
    {
        xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
        xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _waveLogic.xMax2d;
        zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
        zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _waveLogic.yMax2d;

        xVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(0.0f);
        zVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(0.0f);
    }

    private void CheckVariable(float endValue, Vector3 affectedAxis)
    {
        if (!selectedObject) return;
        var currentPos = selectedObject.transform.position;
        if (affectedAxis.x > 0.1)
            currentPos.x =
                _waveLogic.xOrigin2d.position.x +
                _waveLogic.CalcToWorldSpace(endValue);
        if (affectedAxis.z > 0.1)
            currentPos.z =
                _waveLogic.xOrigin2d.position.z +
                _waveLogic.CalcToWorldSpace(endValue);
        selectedObject.transform.position = currentPos;

    }

    public Vector3 GetInputPosition(WaveEquationSelectScript.SelectObjectType type)
    {

        if(type ==  WaveEquationSelectScript.SelectObjectType.SourceSelect)
            return new Vector3(inputXVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(),0.0f,
                inputZVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue());

        return Vector3.zero;
    }

    public Vector3 GetPositionInWorldSpace(WaveEquationSelectScript.SelectObjectType type, bool alwaysReturnWorldCoord = false)
    {
        if (selectedObject != null && selectedObject.type == type)
        {
            return selectedObject.transform.position;
        }

        var pos = _waveLogic.xOrigin2d.position;
        switch (type)
        {
            case WaveEquationSelectScript.SelectObjectType.SourceSelect:
                pos += new Vector3(
                    _waveLogic.CalcToWorldSpace(inputXVariable.GetComponent<PC_InputParser_Float_TMP>()
                        .GetValue()),0,
                    _waveLogic.CalcToWorldSpace(inputZVariable.GetComponent<PC_InputParser_Float_TMP>()
                        .GetValue())
                    ); 
                return pos;
            case WaveEquationSelectScript.SelectObjectType.VisualizationPlaneSelect:
                break;
        }
     
        return Vector3.zero;
    }

    public void DeselectAll()
    {
        if (selectedObject != null)
        {
            //deselect old
            selectedObject.Deselect();
            switch (selectedObject.type)
            {
                case WaveEquationSelectScript.SelectObjectType.SourceSelect:
                    AdaptButtonTextCharge();
                    waveSourceAmplitudeSlider.AllowReset(true);
                    waveSourceLengthSlider.AllowReset(true);
                    waveSourceFrequencySlider.AllowReset(true);
                    waveSourcePhaseSlider.AllowReset(true);
                    break;
                case WaveEquationSelectScript.SelectObjectType.VisualizationPlaneSelect:
                    break;

            }
        }

        selectedObject = null;
    }
    
    public void SelectObject(WaveEquationSelectScript obj)
    {
        if (selectedObject == obj) return;
        DeselectAll();

        //select new
        selectedObject = obj;
        if (selectedObject != null)
        {

            AdaptButtonTextCharge();
            AdaptVariableFields();
            SetSourceVariableFields();
            PositionChanged();

        }
    }

    public void PositionChanged()
    {
        var endPos = Vector3.zero;

        var pos = selectedObject.transform.position;
        var position = _waveLogic.xOrigin2d.position;
        endPos.x = _waveLogic.WorldToCalcSpace(pos.x - position.x);
        endPos.z = _waveLogic.WorldToCalcSpace(pos.z - position.z);

        switch (selectedObject.type)
        {
            case WaveEquationSelectScript.SelectObjectType.SourceSelect:
                inputXVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
                inputZVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);
                SetSourceVariableFields();
                break;
            case WaveEquationSelectScript.SelectObjectType.VisualizationPlaneSelect:
                break;
        }
    }
}