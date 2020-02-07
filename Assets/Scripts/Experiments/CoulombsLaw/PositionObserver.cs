using TMPro;
using UnityEngine;

public class PositionObserver : MonoBehaviour
{
    public GameObject observingGameObject;

    public GameObject xText;
    public GameObject yText;
    public GameObject zText;

    private CoulombLogic _coulombLogic;

    private void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);

        xText.GetComponent<PC_InputParser_Float_TMP>()
            .onValueChangedFloat.AddListener((newVal) => UpdateObjectPosition(newVal, new Vector3(1f, 0f, 0f)));
        yText.GetComponent<PC_InputParser_Float_TMP>()
            .onValueChangedFloat.AddListener((newVal) => UpdateObjectPosition(newVal, new Vector3(0f, 1f, 0f)));
        zText.GetComponent<PC_InputParser_Float_TMP>()
            .onValueChangedFloat.AddListener((newVal) => UpdateObjectPosition(newVal, new Vector3(0f, 0f, 1f)));
    }

    private void UpdateObjectPosition(float newValue, Vector3 axis)
    {
        if (_coulombLogic.IsIn2dMode())
        {
            var newPos = observingGameObject.activeSelf
                ? observingGameObject.transform.position
                : _coulombLogic.xOrigin2d.transform.position;

            if (axis.x > 0.1f)
                newPos.x = _coulombLogic.xOrigin2d.transform.position.x + _coulombLogic.CalcToWorldSpace(newValue);
            if (axis.y > 0.1f)
                newPos.y = _coulombLogic.xOrigin2d.transform.position.y + _coulombLogic.CalcToWorldSpace(newValue);
            if (axis.z > 0.1f)
                newPos.z = _coulombLogic.xOrigin2d.transform.position.z +
                           _coulombLogic.CalcToWorldSpace(newValue); //SHOULD NEVER BE ABLE TO GET IN HERE

            observingGameObject.transform.position = newPos;
            if (!observingGameObject.activeSelf)
                observingGameObject.SetActive(true);
        }
        else
        {
            //WS = WorldSpace
            var newPosWS = observingGameObject.activeSelf
                ? observingGameObject.transform.position : _coulombLogic.xOrigin3d.transform.position;
            if (axis.x > 0.1f)
            {
                var test = _coulombLogic.xOrigin3d.transform.position;
                test.x += _coulombLogic.CalcToWorldSpace(newValue, false);
                newPosWS.x = test.x; //_coulombLogic.xOrigin3d.InverseTransformPoint(test).x;
                
            }
            if (axis.y > 0.1f)
            {
                var test = _coulombLogic.xOrigin3d.transform.position;
                test.y += _coulombLogic.CalcToWorldSpace(newValue, false);
                newPosWS.y = test.y; //_coulombLogic.xOrigin3d.InverseTransformPoint(test).y;
            }
            if (axis.z > 0.1f)
            {
                var test = _coulombLogic.xOrigin3d.transform.position;
                test.z += _coulombLogic.CalcToWorldSpace(newValue, false);
                newPosWS.z = test.z; //_coulombLogic.xOrigin3d.InverseTransformPoint(test).z;
            }

            observingGameObject.transform.position = newPosWS;
            if (!observingGameObject.activeSelf)
                observingGameObject.SetActive(true);
        }
    }


    public void UpdatePosition()
    {
        if (!_coulombLogic)
        {
            var obj = GameObject.Find("CoulombLogic");
            if (obj) _coulombLogic = obj.GetComponent<CoulombLogic>();
        }
        
        Debug.Log("Update Position from Ruler");
        var endPos = Vector3.zero;
        if (_coulombLogic.IsIn2dMode() && observingGameObject.activeSelf)
        {
            var pos = observingGameObject.transform.position;
            var position = _coulombLogic.xOrigin2d.position;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y);
            endPos.z = 0f; //_coulombLogic.WorldToCalcSpace(pos.z - position.z);
        }
        else if (observingGameObject.activeSelf)
        {
            var position = _coulombLogic.xOrigin3d.localPosition;
            var pos =
                _coulombLogic.scene3D.transform.InverseTransformPoint(observingGameObject.transform.position);

            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x, true);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y, true);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z, true);
        }

        xText.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.x);
        yText.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.y);
        zText.GetComponent<PC_TextFormatter_TMP>().FormatString(endPos.z);
    }

    public void OnCoulombModeChanged(bool in3dMode)
    {
        if (!in3dMode)
        {
            xText.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xText.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax2d;
            yText.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yText.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax2d;
            zText.GetComponent<TMP_InputField>().interactable = false;
        }
        else
        {
            xText.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            xText.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.xMax3d;
            yText.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            yText.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.yMax3d;
            zText.GetComponent<TMP_InputField>().interactable = true;
            zText.GetComponent<PC_InputParser_Float_TMP>().minimum = 0f;
            zText.GetComponent<PC_InputParser_Float_TMP>().maximum = _coulombLogic.zMax3d;
        }

        xText.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        yText.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
        zText.GetComponent<PC_TextFormatter_TMP>().FormatString(0f);
    }
}