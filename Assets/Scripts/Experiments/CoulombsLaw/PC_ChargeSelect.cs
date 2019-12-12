using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class PC_ChargeSelect : MonoBehaviour
{
    public GameObject xVariable;
    public GameObject yVariable;
    public GameObject zVariable;

    public GameObject buttonAddDelete;
    public GameObject buttonText;
    
    public PC_SelectScript selectedObject = null;

    private CoulombLogic _coulombLogic;
    
    private void Start()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj) _coulombLogic = obj.GetComponent<CoulombLogic>();
        
        if(_coulombLogic)
            _coulombLogic.onModeChange.AddListener(ChangeMode);
        
        AdaptButtonText();
        AdaptVariableFields();
        xVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckXVariable);
        yVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckYVariable);
        zVariable.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(CheckZVariable);
    }
    
    private void AdaptButtonText()
    {
        Debug.Assert(buttonText && buttonText.GetComponent<LocalizedText_TextMeshPro>());
        buttonText.GetComponent<LocalizedText_TextMeshPro>().key = selectedObject == null ? "Button_Add" : "Button_Delete";
    }

    private void ChangeMode(bool in3dMode)
    {
        if (selectedObject)
        {
            selectedObject.Deselect();
            selectedObject = null;
        }
        
        AdaptButtonText();
        AdaptVariableFields();
    }

    public void AdaptVariableFields()
    {
        if (_coulombLogic.IsIn2dMode())
        {
            xVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax2d;
            yVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = _coulombLogic.yMax2d;
            zVariable.GetComponent<TMP_InputField>().text = "0.00";
            zVariable.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            xVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax3d;
            yVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax3d;
            zVariable.GetComponent<TMP_InputField>().interactable = true;
            zVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            zVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.zMax3d;
        }
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

    public void SelectObject(PC_SelectScript obj)
    {
        selectedObject = obj;
        var endPos = Vector3.zero;
        if (_coulombLogic.IsIn2dMode())
        {
            var pos = obj.transform.position;
            var position = _coulombLogic.xOrigin2d.position;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z);
        }
        else
        {
            var pos = obj.transform.localPosition;
            var position = _coulombLogic.xOrigin3d.localPosition;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x, true);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y, true);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z, true);
        }
        
        xVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
        yVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
        zVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);
    }
    
}
