using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Maroon.reusablePrefabs.NewRuler.Scripts;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using TMPro;
using UnityEngine;
using Maroon.GlobalEntities;

public class Test_RulerDisplay : MonoBehaviour
{
    [SerializeField] private RulerLogic ruler;

    [SerializeField] private TMP_Text DistanceText;

    [SerializeField] private GameObject StartPositionX;
    [SerializeField] private GameObject StartPositionY;
    [SerializeField] private GameObject StartPositionZ;

    [SerializeField] private GameObject EndPositionX;
    [SerializeField] private GameObject EndPositionY;
    [SerializeField] private GameObject EndPositionZ;

    [SerializeField] private GameObject StartPositionXUnit;
    [SerializeField] private GameObject StartPositionYUnit;
    [SerializeField] private GameObject StartPositionZUnit;

    [SerializeField] private GameObject EndPositionXUnit;
    [SerializeField] private GameObject EndPositionYUnit;
    [SerializeField] private GameObject EndPositionZUnit;
    
    private Vector3 startMeasuringPosition;
    private Vector3 endMeasuringPosition;

    public Vector3 StartMeasuringPosition => CoordSystemHelper.Instance.GetSystemPosition(ruler.RulerStart.transform.position);

    public Vector3 EndMeasuringPosition => CoordSystemHelper.Instance.GetSystemPosition(ruler.RulerEnd.transform.position);

    //TODO: Third parameter of CheckVariable should be the select script e.g. ruler.RulerStart.GetComponent<PC_SelectScript>() to invoke coloumbs law change logic.
    private void Start()
    {
        StartPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right , ruler.RulerStart);
        });
        StartPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, ruler.RulerStart);
        });
        StartPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, ruler.RulerStart);
        });

        EndPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right, ruler.RulerEnd);
        });
        EndPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, ruler.RulerEnd);
        });
        EndPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, ruler.RulerEnd);
        });

        List<Unit> subDivUnits = CoordSystemHelper.Instance.GetSubdivisionUnits();

        StartPositionXUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(0));
        StartPositionYUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(1));
        StartPositionZUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(2));

        EndPositionXUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(0));
        EndPositionYUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(1));
        EndPositionZUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(2));
    }

    private void CheckVariable(float endValue, Vector3 affectedAxis, GameObject selectedObject)
    {
        var newPosition = CoordSystemHelper.Instance.CalculateNewWorldPosition(selectedObject.transform.position, endValue, affectedAxis);
        selectedObject.transform.position = newPosition;

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (ruler == null) return;

        if (!ruler.RulerStart.gameObject.activeSelf || !ruler.RulerEnd.gameObject.activeSelf)
        {
            DistanceText.text = "---";
        }
        else
        {
            var displayUnit = CoordSystemHelper.Instance.IsCoordSystemAvailable ? Unit.mm : Unit.m;
            var distance = ruler.CalculateDistance(displayUnit);
            DistanceText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.###} ", distance) + displayUnit.ToString();
        }

        if (!ruler.RulerStart.gameObject.activeSelf)
        {
            StartPositionX.GetComponent<TMP_InputField>().text = "";
            StartPositionY.GetComponent<TMP_InputField>().text = "";
            StartPositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var startPosition = StartMeasuringPosition;
            StartPositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(startPosition.x);
            StartPositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(startPosition.y);
            StartPositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(startPosition.z);
        }

        if (!ruler.RulerEnd.gameObject.activeSelf)
        {
            EndPositionX.GetComponent<TMP_InputField>().text = "";
            EndPositionY.GetComponent<TMP_InputField>().text = "";
            EndPositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var posPosition = EndMeasuringPosition;
            EndPositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.x);
            EndPositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.y);
            EndPositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.z);
        }
    }
}
