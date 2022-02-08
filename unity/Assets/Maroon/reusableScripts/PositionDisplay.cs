using GEAR.Gadgets.Extensions;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using Maroon.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PositionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject affectedObject; 

    [SerializeField] private InputField PositionX;
    [SerializeField] private InputField PositionY;
    [SerializeField] private InputField PositionZ;

    [SerializeField] private TMP_Text PositionXUnit;
    [SerializeField] private TMP_Text PositionYUnit;
    [SerializeField] private TMP_Text PositionZUnit;

    public Vector3 ObjectPosition => CoordSystemHandler.Instance.GetSystemPosition(affectedObject.transform.position);

    public UnityEvent onInputFieldEdited; 

    private void Start()
    {
        PositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal => CheckVariable(endVal, Axis.X));
        PositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal => CheckVariable(endVal, Axis.Y));
        PositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal => CheckVariable(endVal, Axis.Z));

        UpdatePositionUnits();
        UpdateDisplay();
    }

    private void UpdatePositionUnits()
    {
        List<Unit> subDivUnits = CoordSystemHandler.Instance.GetSubdivisionUnits();

        if (PositionXUnit)
            PositionXUnit.GetComponent<TMP_Text>().text = subDivUnits.ElementAt(0).GetStringValue();

        if(PositionYUnit)
            PositionYUnit.GetComponent<TMP_Text>().text = subDivUnits.ElementAt(1).GetStringValue();

        if (PositionZUnit)
            PositionZUnit.GetComponent<TMP_Text>().text = subDivUnits.ElementAt(2).GetStringValue();
    }

    private void CheckVariable(float endValue, Axis affectedAxis)
    {
        var newPosition = DetermineNewPosition(endValue, affectedAxis);
        affectedObject.transform.position = newPosition;
        
        UpdateDisplay();
        onInputFieldEdited.Invoke();
    }

    public void UpdateDisplay()
    {
        if (!affectedObject.activeSelf)
        {
            PositionX.GetComponent<TMP_InputField>().text = "";
            PositionY.GetComponent<TMP_InputField>().text = "";
            PositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var position = ObjectPosition;
            PositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(position.x);
            PositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(position.y);
            PositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(position.z);
        }
    }

    public Vector3 DetermineNewPosition(float value, Axis axis)
    {
        var newPosition = affectedObject.transform.position;
  
        if(axis == Axis.X)
            newPosition.x = CoordSystemHandler.Instance.GetWorldPosition(new Vector3(value, 0, 0)).x;

        if(axis == Axis.Y)
            newPosition.y = CoordSystemHandler.Instance.GetWorldPosition(new Vector3(0, value, 0)).y;

        if (axis == Axis.Z)
            newPosition.z = CoordSystemHandler.Instance.GetWorldPosition(new Vector3(0, 0, value)).z;
       
        return newPosition;
    }
}
