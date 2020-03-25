using System;
using System.Collections.Generic;
using GEAR.Localization;
using PlatformControls.PC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PC_SelectionHandler : MonoBehaviour
{
    [Header("Charge Specific Game Objects")]
    public GameObject ChargeXVariable;

    public GameObject ChargeYVariable;
    public GameObject ChargeZVariable;
    public PC_Slider ChargeValueSlider;
    public Toggle ChargeFixedPosition;

    public GameObject ChargeButtonAddDelete;
    public GameObject ChargeButtonText;
    public GameObject ChargePrefab;
    public UIChargeDragHandler UICharge;

    [Header("General Game Objects")] public PC_RegisterBase selectionRegister;
    public LocalizedTMP nameKey;
    public GameObject xVariablePosition;
    public GameObject yVariablePosition;
    public GameObject zVariablePosition;

    public GameObject xVariableRotation;
    public GameObject yVariableRotation;
    public GameObject zVariableRotation;

    public List<GameObject> rotationParent = new List<GameObject>();

    [Header("Others")] public PC_SelectScript selectedObject = null;
    public Material highlightMaterial;

    private CoulombLogic _coulombLogic;

    private void Start()
    {
        if (!selectionRegister.selectedOnStart)
        {
            selectionRegister.registerHandler.SelectRegister(selectionRegister);
            selectionRegister.registerHandler.DeselectRegister(selectionRegister);
            selectionRegister.gameObject.SetActive(false);
        }

        var obj = GameObject.Find("CoulombLogic");
        if (obj) _coulombLogic = obj.GetComponent<CoulombLogic>();

        if (_coulombLogic)
            _coulombLogic.onModeChange.AddListener(ChangeMode);

        AdaptButtonTextCharge();
        AdaptVariableFieldsCharge();
        ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
                CheckVariable(endVal, Vector3.right);
        });
        ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
                CheckVariable(endVal, Vector3.up);
        });
        ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
                CheckVariable(endVal, Vector3.forward);
        });
        ChargeFixedPosition.onValueChanged.AddListener((isNowFixed) =>
        {
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
            {
                selectedObject.GetComponent<CoulombChargeBehaviour>().SetFixedPosition(isNowFixed);
            }
        });
        ChargeValueSlider.onValueChanged.AddListener((newValue) =>
        {
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
                selectedObject.GetComponent<CoulombChargeBehaviour>().Charge = newValue;
        });


        xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckVariable(endVal, Vector3.right);
        });
        yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat
            .AddListener((endVal) => { CheckVariable(endVal, Vector3.up); });
        zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckVariable(endVal, Vector3.forward);
        });

        xVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckRotation(endVal, Vector3.right);
        });
        yVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat
            .AddListener((endVal) => { CheckRotation(endVal, Vector3.up); });
        zVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener((endVal) =>
        {
            CheckRotation(endVal, Vector3.forward);
        });

        ChargeButtonAddDelete.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("On Click Add Delete");
            if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
            {
                //DELETE
                DeleteSelectedCharge();
            }
            else
            {
                //CREATE
                _coulombLogic.CreateCharge(ChargePrefab, GetInputPosition(PC_SelectScript.SelectType.ChargeSelect),
                    UICharge.ChargeValue, UICharge.FixedPosition, false);
                AdaptButtonTextCharge();
            }
        });

        AdaptVariableFieldsCharge();
        AdaptVariableFields();
    }

    private void DeleteSelectedCharge()
    {
        var coulomb = selectedObject.GetComponent<CoulombChargeBehaviour>();
        SelectObject(null);
        _coulombLogic.RemoveParticle(coulomb, true);
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
        
        if (!Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKeyDown(KeyCode.Escape)) return;
        switch (selectedObject.type)
        {
            case PC_SelectScript.SelectType.ChargeSelect:
                DeleteSelectedCharge();
                break;
            case PC_SelectScript.SelectType.VisualizationPlaneSelect:
                //Nothing happens
                break;
            case PC_SelectScript.SelectType.VoltmeterSelect:
                selectedObject.GetComponent<VoltmeterMeasuringPoint>().HideObject();
                SelectObject(null);
                break;
            case PC_SelectScript.SelectType.CubeSelect:
                //Nothing happens
                break;
            case PC_SelectScript.SelectType.WhiteboardSelect:
                //Nothing happens
                break;
            case PC_SelectScript.SelectType.RulerSelect:
                selectedObject.gameObject.SetActive(false);
                SelectObject(null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void AdaptButtonTextCharge()
    {
        if (ChargeButtonText == null || ChargeButtonText.GetComponent<LocalizedTMP>() == null) return;
        if (!ChargeButtonText) return;
        ChargeButtonText.GetComponent<LocalizedTMP>().Key =
            selectedObject == null || selectedObject.type != PC_SelectScript.SelectType.ChargeSelect
                ? "Button_Add"
                : "Button_Delete";
        ChargeButtonText.transform.parent.GetComponent<PC_Tooltip>().TooltipKey =
            selectedObject == null || selectedObject.type != PC_SelectScript.SelectType.ChargeSelect
                ? "TT_ChargeAdd"
                : "TT_ChargeDelete";
    }

    private void ChangeMode(bool in3dMode)
    {
        if (selectedObject)
        {
            selectedObject.Deselect();
            selectedObject = null;
        }

        AdaptButtonTextCharge();
        AdaptVariableFieldsCharge();
        AdaptVariableFields();
    }

    public void AdaptVariableFieldsCharge()
    {
        if (_coulombLogic.IsIn2dMode())
        {
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax2d;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax2d;
            ChargeZVariable.GetComponent<TMP_InputField>().text = "0.00";
            ChargeZVariable.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax3d;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax3d;
            ChargeZVariable.GetComponent<TMP_InputField>().interactable = true;
            ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.zMax3d;
        }

        if (selectedObject != null && selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
        {
            ChargeValueSlider.resetEnabled = false;
            ChargeValueSlider.SetSliderValue(selectedObject.GetComponent<CoulombChargeBehaviour>().Charge);
            ChargeFixedPosition.isOn = selectedObject.GetComponent<CoulombChargeBehaviour>().fixedPosition;
        }

        ChargeXVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        ChargeYVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        ChargeZVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
    }

    public void AdaptVariableFields()
    {
        if (_coulombLogic.IsIn2dMode())
        {
            xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax2d;
            yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax2d;
            zVariablePosition.GetComponent<TMP_InputField>().text = "0.00";
            zVariablePosition.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax3d;
            yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax3d;
            zVariablePosition.GetComponent<TMP_InputField>().interactable = true;
            zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.zMax3d;
        }

        xVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        yVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        zVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
    }

    private void CheckVariable(float endValue, Vector3 affectedAxis)
    {
        if (!selectedObject) return;
        if (_coulombLogic.IsIn2dMode())
        {
            var currentPos = selectedObject.transform.position;
            if (affectedAxis.x > 0.1)
                currentPos.x =
                    _coulombLogic.xOrigin2d.position.x +
                    _coulombLogic.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            if (affectedAxis.y > 0.1)
                currentPos.y =
                    _coulombLogic.xOrigin2d.position.y +
                    _coulombLogic.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            if (affectedAxis.z > 0.1)
                currentPos.z =
                    _coulombLogic.xOrigin2d.position.z +
                    _coulombLogic.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            selectedObject.transform.position = currentPos;
        }
        else
        {
            var currentPos = selectedObject.transform.localPosition;
            if (affectedAxis.x > 0.1)
                currentPos.x = _coulombLogic.xOrigin3d.localPosition.x +
                               _coulombLogic.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            if (affectedAxis.y > 0.1)
                currentPos.y = _coulombLogic.xOrigin3d.localPosition.y +
                               _coulombLogic.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            if (affectedAxis.z > 0.1)
                currentPos.z = _coulombLogic.xOrigin3d.localPosition.z +
                               _coulombLogic.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            selectedObject.transform.localPosition = currentPos;
        }
    }

    public Vector3 GetInputPosition(PC_SelectScript.SelectType type)
    {
        switch (type)
        {
            case PC_SelectScript.SelectType.ChargeSelect:
                return new Vector3(ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(),
                    ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(),
                    ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue());
            case PC_SelectScript.SelectType.VoltmeterSelect:
            case PC_SelectScript.SelectType.VisualizationPlaneSelect:
                return new Vector3(xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue(),
                    yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue(),
                    zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue());
        }

        return Vector3.zero;
    }

    public Vector3 GetPositionInWorldSpace(PC_SelectScript.SelectType type, bool alwaysReturnWorldCoord = false)
    {
        if (selectedObject != null && selectedObject.type == type)
        {
            return selectedObject.transform.position;
        }

        if (_coulombLogic.IsIn2dMode())
        {
            var pos = _coulombLogic.xOrigin2d.position;
            switch (type)
            {
                case PC_SelectScript.SelectType.ChargeSelect:
                    pos += new Vector3(
                        _coulombLogic.CalcToWorldSpace(ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        _coulombLogic.CalcToWorldSpace(ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        0); //not supported in 2d -> maybe change that
                    return pos;
                case PC_SelectScript.SelectType.VisualizationPlaneSelect:
                case PC_SelectScript.SelectType.VoltmeterSelect:
                    pos += new Vector3(
                        _coulombLogic.CalcToWorldSpace(xVariablePosition.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        _coulombLogic.CalcToWorldSpace(yVariablePosition.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        0); //not supported in 2d -> maybe change that
                    return pos;
            }
        }
        else
        {
            var pos = _coulombLogic.xOrigin3d.localPosition;
            switch (type)
            {
                case PC_SelectScript.SelectType.ChargeSelect:
                    return pos + new Vector3(
                               _coulombLogic.CalcToWorldSpace(
                                   ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               _coulombLogic.CalcToWorldSpace(
                                   ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               _coulombLogic.CalcToWorldSpace(
                                   ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true));
                default:
                    return pos + new Vector3(
                               _coulombLogic.CalcToWorldSpace(
                                   xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               _coulombLogic.CalcToWorldSpace(
                                   yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               _coulombLogic.CalcToWorldSpace(
                                   zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true));
            }
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
                case PC_SelectScript.SelectType.ChargeSelect:
                    AdaptButtonTextCharge();
                    AdaptVariableFieldsCharge();
                    ChargeValueSlider.resetEnabled = true;
                    break;
                case PC_SelectScript.SelectType.VisualizationPlaneSelect:
                case PC_SelectScript.SelectType.VoltmeterSelect:
                    selectionRegister.registerHandler.DeselectRegister(selectionRegister);
                    selectionRegister.gameObject.SetActive(false);
                    break;
            }
        }

        selectedObject = null;
    }
    
    public void SelectObject(PC_SelectScript obj)
    {
        if (selectedObject == obj) return;
        DeselectAll();

        //select new
        selectedObject = obj;
        if (selectedObject != null)
        {
            if (selectedObject.type == PC_SelectScript.SelectType.VisualizationPlaneSelect ||
                selectedObject.type == PC_SelectScript.SelectType.VoltmeterSelect)
            {
                selectionRegister.gameObject.SetActive(true);
                selectionRegister.registerHandler.SelectRegister(selectionRegister);
            }

            PositionChanged();

            AdaptButtonTextCharge();
            AdaptVariableFieldsCharge();
            AdaptVariableFields();
        }
    }

    public void PositionChanged()
    {
        var endPos = Vector3.zero;
        if (_coulombLogic.IsIn2dMode())
        {
            var pos = selectedObject.transform.position;
            var position = _coulombLogic.xOrigin2d.position;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y);
            endPos.z = 0f; //_coulombLogic.WorldToCalcSpace(pos.z - position.z);
        }
        else
        {
            var pos = selectedObject.transform.localPosition;
            var position = _coulombLogic.xOrigin3d.localPosition;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x, true);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y, true);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z, true);
        }

        switch (selectedObject.type)
        {
            case PC_SelectScript.SelectType.ChargeSelect:
                ChargeXVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
                ChargeYVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
                ChargeZVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);
                break;
            case PC_SelectScript.SelectType.VisualizationPlaneSelect:
            case PC_SelectScript.SelectType.VoltmeterSelect:
                nameKey.Key = selectedObject.nameKey;
                nameKey.UpdateLocalizedText();
                xVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
                yVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
                zVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);

                foreach (var rot in rotationParent)
                {
                    rot.SetActive(selectedObject.type == PC_SelectScript.SelectType.VisualizationPlaneSelect);
                }

                if (selectedObject.type == PC_SelectScript.SelectType.VisualizationPlaneSelect)
                    RotationChanged();

                break;
        }
    }

    public void RotationChanged()
    {
        var rotation = selectedObject.transform.localRotation.eulerAngles;
        xVariableRotation.GetComponent<PC_TextFormatter_TMP>()
            .FormatString(rotation.x <= 180f ? rotation.x : -Mathf.Abs(360f - rotation.x));
        yVariableRotation.GetComponent<PC_TextFormatter_TMP>()
            .FormatString(rotation.y <= 180f ? rotation.y : -Mathf.Abs(360f - rotation.y));
        zVariableRotation.GetComponent<PC_TextFormatter_TMP>()
            .FormatString(rotation.z <= 180f ? rotation.z : -Mathf.Abs(360f - rotation.z));
    }

    private void CheckRotation(float endValue, Vector3 axis)
    {
        if (!selectedObject || selectedObject.type != PC_SelectScript.SelectType.VisualizationPlaneSelect) return;
        var currentRot = selectedObject.transform.localRotation.eulerAngles;
        selectedObject.transform.localRotation = Quaternion.Euler(axis.x > 0.1 ? endValue : currentRot.x,
            axis.y > 0.1f ? endValue : currentRot.y, axis.z > 0.1f ? endValue : currentRot.z);
    }
}