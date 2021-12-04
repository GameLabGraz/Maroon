using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Maroon.reusablePrefabs.NewRuler.Scripts;
using Maroon.Physics.CoordinateSystem;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    private Vector3 startMeasuringPosition => CoordSystem.Instance.GetPositionInAxisUnits(ruler.RulerStart.transform.position);
    private Vector3 endMeasuringPosition => CoordSystem.Instance.GetPositionInAxisUnits(ruler.RulerEnd.transform.position);

    private void Start()
    {
        StartPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right, ruler.RulerStart.GetComponent<PC_SelectScript>());
        });
        StartPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, ruler.RulerStart.GetComponent<PC_SelectScript>());
        });
        StartPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, ruler.RulerStart.GetComponent<PC_SelectScript>());
        });

        EndPositionX.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.right, ruler.RulerEnd.GetComponent<PC_SelectScript>());
        });
        EndPositionY.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.up, ruler.RulerEnd.GetComponent<PC_SelectScript>());
        });
        EndPositionZ.GetComponent<PC_InputParser_Float_TMP>().onValueChangedFloat.AddListener(endVal =>
        {
            CheckVariable(endVal, Vector3.forward, ruler.RulerEnd.GetComponent<PC_SelectScript>());
        });

        var subDivUnits = CoordSystem.Instance.GetAxisSubDivisionUnits();

        StartPositionXUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(0));
        StartPositionYUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(1));
        StartPositionZUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(2));

        EndPositionXUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(0));
        EndPositionYUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(1));
        EndPositionZUnit.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Unit), subDivUnits.ElementAt(2));

        //StartPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();
        //EndPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();


        /* CoulombLogic.Instance.onModeChange.AddListener(in3dMode =>
         {
             StartPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
             EndPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
         });*/
    }

    private static void CheckVariable(float endValue, Vector3 affectedAxis, PC_SelectScript selectedObject)
    {
        if (!selectedObject) return;


        /* if (CoulombLogic.Instance.IsIn2dMode())
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
             var currentPos = CoulombLogic.Instance.scene3D.transform.InverseTransformPoint(selectedObject.transform.position);

             if (affectedAxis.x > 0.1)
                 currentPos.x = CoulombLogic.Instance.xOrigin3d.localPosition.x +
                                CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
             if (affectedAxis.y > 0.1)
                 currentPos.y = CoulombLogic.Instance.xOrigin3d.localPosition.y +
                                CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1
             if (affectedAxis.z > 0.1)
                 currentPos.z = CoulombLogic.Instance.xOrigin3d.localPosition.z +
                                CoulombLogic.Instance.CalcToWorldSpace(endValue, true); //end Value is between 0 and 1

             selectedObject.transform.position = CoulombLogic.Instance.scene3D.transform.TransformPoint(currentPos);

             selectedObject.onPositionChanged.Invoke(currentPos);
         }*/
    }

    void Update()
    {
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
            //var distance = Vector3.Distance(startMeasuringPosition, endMeasuringPosition);
            var distance = ruler.CalculateDistance();
            //TODO ADD unit
            DistanceText.text = string.Format(CultureInfo.InvariantCulture, "{0:0.###} cm", distance);
        }


        if (!ruler.RulerStart.gameObject.activeSelf)
        {
            StartPositionX.GetComponent<TMP_InputField>().text = "";
            StartPositionY.GetComponent<TMP_InputField>().text = "";
            StartPositionZ.GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            var startPosition = startMeasuringPosition;
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
            var posPosition = endMeasuringPosition;
            EndPositionX.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.x);
            EndPositionY.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.y);
            EndPositionZ.GetComponent<PC_TextFormatter_TMP>().FormatString(posPosition.z);
        }
    }
}
