using System;
using GEAR.Localization.Text;
using PlatformControls.PC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

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

    [Header("Others")] public PC_SelectScript selectedObject = null;
    public Material highlightMaterial;

    private void Start()
    {
        CoulombLogic.Instance.onModeChange.AddListener(ChangeMode);

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
                CoulombLogic.Instance.CreateCharge(ChargePrefab, GetInputPosition(PC_SelectScript.SelectType.ChargeSelect),
                    UICharge.ChargeValue, UICharge.FixedPosition, false);
                AdaptButtonTextCharge();
            }
        });

        AdaptVariableFieldsCharge();
    }

    private void DeleteSelectedCharge()
    {
        var coulomb = selectedObject.GetComponent<CoulombChargeBehaviour>();
        SelectObject(null);
        CoulombLogic.Instance.RemoveParticle(coulomb, true);
        AdaptButtonTextCharge();
    }

    private void Update()
    {
        if (selectedObject == null)
        {
            AdaptButtonTextCharge();
            return;
        }

        if (!selectedObject.isActiveAndEnabled)
        {
            SelectObject(null);
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

        var tooltip = ChargeButtonText.transform.parent.GetComponent<PC_Tooltip>();
        if (tooltip != null)
        {
            tooltip.TooltipKey =
                selectedObject == null || selectedObject.type != PC_SelectScript.SelectType.ChargeSelect
                    ? "TT_ChargeAdd"
                    : "TT_ChargeDelete";
        }
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
        if (CoulombLogic.Instance.IsIn2dMode())
        {
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = CoulombLogic.Instance.xMax2d;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = CoulombLogic.Instance.yMax2d;
            ChargeZVariable.GetComponent<TMP_InputField>().text = "0.00";
            ChargeZVariable.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = CoulombLogic.Instance.xMax3d;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = CoulombLogic.Instance.yMax3d;
            ChargeZVariable.GetComponent<TMP_InputField>().interactable = true;
            ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().maximum = CoulombLogic.Instance.zMax3d;
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

    private void CheckVariable(float endValue, Vector3 affectedAxis)
    {
        if (!selectedObject) return;
        if (CoulombLogic.Instance.IsIn2dMode())
        {
            var currentPos = selectedObject.transform.position;
            if (affectedAxis.x > 0.1)
                currentPos.x =
                    CoulombLogic.Instance.xOrigin2d.position.x +
                    CoulombLogic.Instance.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            if (affectedAxis.y > 0.1)
                currentPos.y =
                    CoulombLogic.Instance.xOrigin2d.position.y +
                    CoulombLogic.Instance.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            if (affectedAxis.z > 0.1)
                currentPos.z =
                    CoulombLogic.Instance.xOrigin2d.position.z +
                    CoulombLogic.Instance.CalcToWorldSpace(endValue); //end Value is between 0 and 1
            selectedObject.transform.position = currentPos;

            selectedObject.onPositionChanged.Invoke(currentPos);
        }
        else
        {
            var currentPos = selectedObject.transform.localPosition;
            if (affectedAxis.x > 0.1)
                currentPos.x = CoulombLogic.Instance.xOrigin3d.localPosition.x +
                               CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            if (affectedAxis.y > 0.1)
                currentPos.y = CoulombLogic.Instance.xOrigin3d.localPosition.y +
                               CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            if (affectedAxis.z > 0.1)
                currentPos.z = CoulombLogic.Instance.xOrigin3d.localPosition.z +
                               CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
            selectedObject.transform.localPosition = currentPos;

            selectedObject.onPositionChanged.Invoke(currentPos);
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
        }

        return Vector3.zero;
    }

    public Vector3 GetPositionInWorldSpace(PC_SelectScript.SelectType type, bool alwaysReturnWorldCoord = false)
    {
        if (selectedObject != null && selectedObject.type == type)
        {
            return selectedObject.transform.position;
        }

        if (CoulombLogic.Instance.IsIn2dMode())
        {
            var pos = CoulombLogic.Instance.xOrigin2d.position;
            switch (type)
            {
                case PC_SelectScript.SelectType.ChargeSelect:
                    pos += new Vector3(
                        CoulombLogic.Instance.CalcToWorldSpace(ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        CoulombLogic.Instance.CalcToWorldSpace(ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>()
                            .GetValue()),
                        0); //not supported in 2d -> maybe change that
                    return pos;
            }
        }
        else
        {
            var pos = CoulombLogic.Instance.xOrigin3d.localPosition;
            switch (type)
            {
                case PC_SelectScript.SelectType.ChargeSelect:
                    return pos + new Vector3(
                               CoulombLogic.Instance.CalcToWorldSpace(
                                   ChargeXVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               CoulombLogic.Instance.CalcToWorldSpace(
                                   ChargeYVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true),
                               CoulombLogic.Instance.CalcToWorldSpace(
                                   ChargeZVariable.GetComponent<PC_InputParser_Float_TMP>().GetValue(), true));
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
            }
        }

        selectedObject = null;
    }

    public void ForceResetChargePositions()
    {
        ChargeButtonText.GetComponent<LocalizedTMP>().Key = "Button_Add";
        ChargeButtonText.transform.parent.GetComponent<PC_Tooltip>().TooltipKey = "TT_ChargeAdd";

        ChargeValueSlider.SetSliderValue(0);
        ChargeFixedPosition.isOn = false;

        ChargeXVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        ChargeYVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        ChargeZVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
    }

    public void SelectObject(PC_SelectScript obj)
    {
        if (selectedObject == obj
            || (!CoulombLogic.Instance.chargeInteractionAllowed && obj != null && obj.type == PC_SelectScript.SelectType.ChargeSelect))
            return;
        DeselectAll();

        //select new
        selectedObject = obj;
        if (selectedObject != null)
        {
            AdaptButtonTextCharge();
            AdaptVariableFieldsCharge();

            PositionChanged();
        }
    }

    public void PositionChanged()
    {
        var endPos = Vector3.zero;
        if (CoulombLogic.Instance.IsIn2dMode())
        {
            var pos = selectedObject.transform.position;
            var position = CoulombLogic.Instance.xOrigin2d.position;
            endPos.x = CoulombLogic.Instance.WorldToCalcSpace(pos.x - position.x);
            endPos.y = CoulombLogic.Instance.WorldToCalcSpace(pos.y - position.y);
            endPos.z = 0f; //CoulombLogic.Instance.WorldToCalcSpace(pos.z - position.z);
        }
        else
        {
            var pos = selectedObject.transform.localPosition;
            var position = CoulombLogic.Instance.xOrigin3d.localPosition;
            endPos.x = CoulombLogic.Instance.WorldToCalcSpace(pos.x - position.x, true);
            endPos.y = CoulombLogic.Instance.WorldToCalcSpace(pos.y - position.y, true);
            endPos.z = CoulombLogic.Instance.WorldToCalcSpace(pos.z - position.z, true);
        }

        switch (selectedObject.type)
        {
            case PC_SelectScript.SelectType.ChargeSelect:
                ChargeXVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
                ChargeYVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
                ChargeZVariable.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);
                break;
        }
    }

    private void CheckRotation(float endValue, Vector3 axis)
    {
        if (!selectedObject || selectedObject.type != PC_SelectScript.SelectType.VisualizationPlaneSelect) return;
        var currentRot = selectedObject.transform.localRotation.eulerAngles;
        var eulerRot = new Vector3(axis.x > 0.1 ? endValue : currentRot.x,
            axis.y > 0.1f ? endValue : currentRot.y, axis.z > 0.1f ? endValue : currentRot.z);
        selectedObject.transform.localRotation = Quaternion.Euler(eulerRot);

        selectedObject.onRotationChanged.Invoke(eulerRot);
    }
}