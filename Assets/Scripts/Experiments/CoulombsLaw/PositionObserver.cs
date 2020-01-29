using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionObserver : MonoBehaviour
{
    public GameObject observingGameObject;

    public PC_TextFormatter_TMP xText;
    public PC_TextFormatter_TMP yText;
    public PC_TextFormatter_TMP zText;

    private CoulombLogic _coulombLogic;
    
    private void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
    }


    public void UpdatePosition()
    {
        Debug.Log("Update Position from Ruler");
        var endPos = Vector3.zero;
        if (_coulombLogic.IsIn2dMode() && observingGameObject.activeSelf)
        {
            var pos = observingGameObject.transform.position;
            var position = _coulombLogic.xOrigin2d.position;
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z);
        }
        else if(observingGameObject.activeSelf)
        {
            var position = _coulombLogic.xOrigin3d.localPosition;
            var pos =
                _coulombLogic.scene3D.transform.InverseTransformPoint(observingGameObject.transform.position);
            
            endPos.x = _coulombLogic.WorldToCalcSpace(pos.x - position.x, true);
            endPos.y = _coulombLogic.WorldToCalcSpace(pos.y - position.y, true);
            endPos.z = _coulombLogic.WorldToCalcSpace(pos.z - position.z, true);
        }
        
        xText.FormatString(endPos.x);
        yText.FormatString(endPos.y);
        zText.FormatString(endPos.z);
    }
}
