using System.Globalization;
using Maroon.Physics;
using TMPro;
using UnityEngine;

public class VoltmeterDifferences : MonoBehaviour
{
    public VoltmeterMeasuringPoint positiveMeasuringPoint;
    public VoltmeterMeasuringPoint negativeMeasuringPoint;

    public TMP_Text VoltageText;
    public TMP_Text DistanceText;

    public GameObject NegPositionX;
    public GameObject NegPositionY;
    public GameObject NegPositionZ;

    public GameObject PosPositionX;
    public GameObject PosPositionY;
    public GameObject PosPositionZ;

    public bool onPerDefault = true;
    public bool showUnitInText = true;

    [Header("Assessment System")]
    public QuantityFloat currentValue = 0;
    public QuantityBool voltmeterEnabled = true;

    private bool _isOn;

    private Vector3 positiveMeasuringPosition => GetMeasuringPosition(positiveMeasuringPoint);
    private Vector3 negativeMeasuringPosition => GetMeasuringPosition(negativeMeasuringPoint);

    private Vector3 GetMeasuringPosition(VoltmeterMeasuringPoint measuringPoint)
    {
        if (CoulombLogic.Instance.IsIn2dMode())
        {
            var pos = measuringPoint.transform.position;
            var position = CoulombLogic.Instance.xOrigin2d.position;
            return new Vector3(CoulombLogic.Instance.WorldToCalcSpace(pos.x - position.x),
                CoulombLogic.Instance.WorldToCalcSpace(pos.y - position.y),
                0);
        }
        else
        {
            var pos = measuringPoint.transform.localPosition;
            var position = CoulombLogic.Instance.xOrigin3d.localPosition;
            return new Vector3(CoulombLogic.Instance.WorldToCalcSpace(pos.x - position.x, true),
                CoulombLogic.Instance.WorldToCalcSpace(pos.y - position.y, true),
                CoulombLogic.Instance.WorldToCalcSpace(pos.z - position.z, true));
        }
    }

    private float MeasuringPointDistance => CoulombLogic.Instance.WorldToCalcSpace(Vector3.Distance(positiveMeasuringPoint.transform.position, negativeMeasuringPoint.transform.position));


    private void Start()
    {
        NegPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right, negativeMeasuringPoint.GetComponent<PC_SelectScript>());
        });
        NegPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, negativeMeasuringPoint.GetComponent<PC_SelectScript>());
        });
        NegPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, negativeMeasuringPoint.GetComponent<PC_SelectScript>());
        });

        PosPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right, positiveMeasuringPoint.GetComponent<PC_SelectScript>());
        });
        PosPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, positiveMeasuringPoint.GetComponent<PC_SelectScript>());
        });
        PosPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, positiveMeasuringPoint.GetComponent<PC_SelectScript>());
        });


        NegPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();
        PosPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();



        CoulombLogic.Instance.onModeChange.AddListener(in3dMode =>
        {
            NegPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
            PosPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
        });

        _isOn = onPerDefault;
    }

    private static void CheckVariable(float endValue, Vector3 affectedAxis, PC_SelectScript selectedObject)
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

    public void UpdateDisplay()
    {
        if (positiveMeasuringPoint == null || negativeMeasuringPoint == null) return;

        if (!_isOn || (!positiveMeasuringPoint.gameObject.activeSelf || !negativeMeasuringPoint.gameObject.activeSelf))
        {
            VoltageText.text = "---";
            DistanceText.text = "---";
        }
        else
        {
            VoltageText.text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", 
                GetDifference(), (showUnitInText ? " " + GetCurrentUnit() : ""));
            DistanceText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.###} m", MeasuringPointDistance);
        }


        if (!negativeMeasuringPoint.gameObject.activeSelf)
        {
            if (NegPositionX != null) NegPositionX.GetComponent<TMP_InputField>().text = "";
            if (NegPositionY != null) NegPositionY.GetComponent<TMP_InputField>().text = "";
            if (NegPositionZ != null) NegPositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var negPosition = negativeMeasuringPosition;
            if(NegPositionX != null) NegPositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(negPosition.x);
            if(NegPositionY != null) NegPositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(negPosition.y);
            if(NegPositionZ != null) NegPositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(negPosition.z);
        }

        if (!positiveMeasuringPoint.gameObject.activeSelf)
        {
            if (PosPositionX != null) PosPositionX.GetComponent<TMP_InputField>().text = "";
            if (PosPositionY != null) PosPositionY.GetComponent<TMP_InputField>().text = "";
            if (PosPositionZ != null) PosPositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var posPosition = positiveMeasuringPosition;
            if (PosPositionX != null) PosPositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.x);
            if (PosPositionY != null) PosPositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.y);
            if (PosPositionZ != null) PosPositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.z);
        }
    }

    private string GetDifference()
    {
        var currentDifference = positiveMeasuringPoint.GetPotentialInMicroVolt() -
                                negativeMeasuringPoint.GetPotentialInMicroVolt();
        
        if(Mathf.Abs(currentDifference - currentValue.Value) > 0.000001)
            currentValue.Value = currentDifference;
        return GetCurrentFormattedString();
    }

    private string GetCurrentFormattedString()
    {
        var check = currentValue.Value;
        for (var cnt = 0; Mathf.Abs(check) < 1f && cnt < 2; ++cnt)
        {
            check *= Mathf.Pow(10, 3);
        }
        
        return check.ToString("F3", CultureInfo.InvariantCulture);   
    }

    private string GetCurrentUnit()
    {
        var unit = "V";
        var check = Mathf.Abs(currentValue.Value);
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

    public IQuantity GetVoltage()
    {
        return currentValue;
    }

    public void Enable(bool enable)
    {
        if (!enable)
        {
            positiveMeasuringPoint.HideObject();
            negativeMeasuringPoint.HideObject();
        }
        else {
            //do nothing the user just has to pull in the voltmeter measuring points and now he can
        }
    }

    public void InvokeValueChangedEvent()
    {
        UpdateDisplay();
        
        if (!positiveMeasuringPoint.isActiveAndEnabled && !negativeMeasuringPoint.isActiveAndEnabled)
            currentValue.Value = 0f;
        else
            GetDifference(); //call to update the current value, seems as otherwise this will be done too late
        currentValue.SendValueChangedEvent();
    }

    public void AddValueChangedEventsToCharge(CoulombChargeBehaviour charge)
    {
        if (!charge) return;

        charge.charge.onValueChanged.AddListener(newVal => InvokeValueChangedEvent());
    }
}
