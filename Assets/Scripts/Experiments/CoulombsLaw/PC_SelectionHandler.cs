using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Localization;
using TMPro;
using UnityEngine;

public class PC_SelectionHandler : MonoBehaviour
{
    [Header("Charge Specific Game Objects")]
    public GameObject ChargeXVariable;
    public GameObject ChargeYVariable;
    public GameObject ChargeZVariable;
    
    public GameObject ChargeButtonAddDelete;
    public GameObject ChargeButtonText;

    [Header("General Game Objects")] 
    public PC_RegisterBase selectionRegister;
    public LocalizedText_TextMeshPro nameKey;
    public GameObject xVariablePosition;
    public GameObject yVariablePosition;
    public GameObject zVariablePosition;
    
    public GameObject xVariableRotation;
    public GameObject yVariableRotation;
    public GameObject zVariableRotation;
    
    public List<GameObject> rotationParent = new List<GameObject>();
    
    [Header("Others")]
    public PC_SelectScript selectedObject = null;

    private CoulombLogic _coulombLogic;
    
    private void Start()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj) _coulombLogic = obj.GetComponent<CoulombLogic>();
        
        if(_coulombLogic)
            _coulombLogic.onModeChange.AddListener(ChangeMode);
        
        AdaptButtonTextCharge();
        AdaptVariableFieldsCharge();
        ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckXVarCharge);
        ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckYVarCharge);
        ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckZVarCharge);
        
        xVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckXVariable);
        yVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckYVariable);
        zVariablePosition.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckZVariable);
        
        xVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckXVariableRotation);
        yVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckYVariableRotation);
        zVariableRotation.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckZVariableRotation);
    }
    
    private void AdaptButtonTextCharge()
    {
        Debug.Assert(ChargeButtonText && ChargeButtonText.GetComponent<LocalizedText_TextMeshPro>());
        ChargeButtonText.GetComponent<LocalizedText_TextMeshPro>().key = selectedObject == null || selectedObject.type != PC_SelectScript.SelectType.ChargeSelect? "Button_Add" : "Button_Delete";
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
    }

    public void AdaptVariableFieldsCharge()
    {
        if (_coulombLogic.IsIn2dMode())
        {
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax2d;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = _coulombLogic.yMax2d;
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
    }

    public void CheckXVarCharge(float endValue)
    {
        if (selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
            CheckXVariable(endValue);
    }

    public void CheckYVarCharge(float endValue)
    {
        if (selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
            CheckYVariable(endValue);
    }

    public void CheckZVarCharge(float endValue)
    {
        if (selectedObject.type == PC_SelectScript.SelectType.ChargeSelect)
            CheckZVariable(endValue);
    }
    
    public void CheckXVariable(float endValue)
    {
        if (!selectedObject) return;
        Debug.Log("Float Val: " + endValue);
        if (_coulombLogic.IsIn2dMode())
        {
            var currentPos = selectedObject.transform.position;
            //end Value is between 0 and 1
            currentPos.x = _coulombLogic.xOrigin2d.position.x + _coulombLogic.CalcToWorldSpace(endValue);
            selectedObject.transform.position = currentPos;
        }
        else
        {
            var currentPos = selectedObject.transform.localPosition;
            currentPos.x = _coulombLogic.xOrigin3d.localPosition.x + _coulombLogic.CalcToWorldSpace(endValue, true);
            selectedObject.transform.localPosition = currentPos;
        }
    }

    public void CheckYVariable(float endValue)
    {
        if (!selectedObject) return;
        Debug.Log("Float Val: " + endValue);
        if (_coulombLogic.IsIn2dMode())
        {
            var currentPos = selectedObject.transform.position;
            //end Value is between 0 and 1
            currentPos.y = _coulombLogic.xOrigin2d.position.y + _coulombLogic.CalcToWorldSpace(endValue);
            selectedObject.transform.position = currentPos;
        }
        else
        {
            var currentPos = selectedObject.transform.localPosition;
            currentPos.y = _coulombLogic.xOrigin3d.localPosition.y + _coulombLogic.CalcToWorldSpace(endValue, true);
            selectedObject.transform.localPosition = currentPos;
        }
    }

    public void CheckZVariable(float endValue)
    {
        if (!selectedObject) return;
        if (_coulombLogic.IsIn2dMode())
        {
            //No z variable is supported
        }
        else
        {
            var currentPos = selectedObject.transform.localPosition;
            currentPos.z = _coulombLogic.xOrigin3d.localPosition.z + _coulombLogic.CalcToWorldSpace(endValue, true);
            selectedObject.transform.localPosition = currentPos;
        }
    }

    private void CheckXVariableRotation(float endValue)
    {
        if (!selectedObject || selectedObject.type != PC_SelectScript.SelectType.VisualizationPlaneSelect) return;
        var currentRot = selectedObject.transform.localRotation.eulerAngles;
        selectedObject.transform.localRotation = Quaternion.Euler(endValue, currentRot.y, currentRot.z);
    }

    private void CheckYVariableRotation(float endValue)
    {
        if (!selectedObject || selectedObject.type != PC_SelectScript.SelectType.VisualizationPlaneSelect) return;
        var currentRot = selectedObject.transform.localRotation.eulerAngles;
        selectedObject.transform.localRotation = Quaternion.Euler(currentRot.x, endValue, currentRot.z);
    }
    
    private void CheckZVariableRotation(float endValue)
    {
        if (!selectedObject || selectedObject.type != PC_SelectScript.SelectType.VisualizationPlaneSelect) return;
        var currentRot = selectedObject.transform.localRotation.eulerAngles;
        selectedObject.transform.localRotation = Quaternion.Euler(currentRot.x, currentRot.y, endValue);
    }

    public void SelectObject(PC_SelectScript obj)
    {
        Debug.Log("Select now: " + obj.nameKey);
        if (selectedObject == obj) return;
        if (selectedObject != null)
        {
            //deselect old
            selectedObject.Deselect();
            switch (selectedObject.type)
            {
                case PC_SelectScript.SelectType.ChargeSelect:
                    AdaptButtonTextCharge();
                    AdaptVariableFieldsCharge();
                    break;
                default:
                    selectionRegister.registerHandler.DeselectRegister(selectionRegister);
                    break;
            }
        }

        //select new
        selectedObject = obj;
        if (selectedObject != null)
        {
            if (selectedObject.type != PC_SelectScript.SelectType.ChargeSelect)
                selectionRegister.registerHandler.SelectRegister(selectionRegister);
            PositionChanged();
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
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z);
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
            default:
                nameKey.key = selectedObject.nameKey;
                nameKey.UpdateLocalizedText();
                xVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
                yVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
                zVariablePosition.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);

                foreach (var rot in rotationParent)
                {
                    rot.SetActive(selectedObject.type == PC_SelectScript.SelectType.VisualizationPlaneSelect);
                }
                
                if(selectedObject.type == PC_SelectScript.SelectType.VisualizationPlaneSelect)
                    RotationChanged();
                
                break;
        }
    }

    public void RotationChanged()
    {
        var rotation = selectedObject.transform.localRotation.eulerAngles;
        xVariableRotation.GetComponent<PC_TextFormatter_TMP>().FormatString(rotation.x <= 180f? rotation.x : - Mathf.Abs(360f - rotation.x));
        yVariableRotation.GetComponent<PC_TextFormatter_TMP>().FormatString(rotation.y <= 180f? rotation.y : - Mathf.Abs(360f - rotation.y));
        zVariableRotation.GetComponent<PC_TextFormatter_TMP>().FormatString(rotation.z <= 180f? rotation.z : - Mathf.Abs(360f - rotation.z));
    }

}
