using TMPro;
using UnityEngine;

public class RulerDisplay : MonoBehaviour
{
    [SerializeField] private RulerPrefab ruler;

    [SerializeField] private TMP_Text DistanceText;

    [SerializeField] private GameObject StartPositionX;
    [SerializeField] private GameObject StartPositionY;
    [SerializeField] private GameObject StartPositionZ;

    [SerializeField] private GameObject EndPositionX;
    [SerializeField] private GameObject EndPositionY;
    [SerializeField] private GameObject EndPositionZ;

    private Vector3 startMeasuringPosition => GetMeasuringPosition(ruler.RulerStart);
    private Vector3 endMeasuringPosition => GetMeasuringPosition(ruler.RulerEnd);

    private Vector3 GetMeasuringPosition(GameObject measuringPoint)
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
            var position = CoulombLogic.Instance.xOrigin3d.localPosition;
            var pos = CoulombLogic.Instance.scene3D.transform.InverseTransformPoint(measuringPoint.transform.position);

            return new Vector3(CoulombLogic.Instance.WorldToCalcSpace(pos.x - position.x, true),
                CoulombLogic.Instance.WorldToCalcSpace(pos.y - position.y, true),
                CoulombLogic.Instance.WorldToCalcSpace(pos.z - position.z, true));
        }
    }

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


        StartPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();
        EndPositionZ.GetComponent<TMP_InputField>().interactable = CoulombLogic.Instance.IsIn3dMode();


        CoulombLogic.Instance.onModeChange.AddListener(in3dMode =>
        {
            StartPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
            EndPositionZ.GetComponent<TMP_InputField>().interactable = in3dMode;
        });
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
        }
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
            var distance = Vector3.Distance(startMeasuringPosition, endMeasuringPosition);
            DistanceText.text = $"{distance:0.###} m";
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
