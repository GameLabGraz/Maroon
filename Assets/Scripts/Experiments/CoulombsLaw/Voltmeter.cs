using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltmeter : MonoBehaviour, IResetWholeObject
{


    private CoulombLogic _coulombLogic;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnResetMovingArrows(bool in3dMode)
    {
        if (in3dMode) gameObject.transform.parent = _coulombLogic.scene3D.transform;
        else gameObject.transform.parent = _coulombLogic.scene2D.transform;

            var movArrows = GetComponentInChildren<PC_ArrowMovement>();
        if (!movArrows) return;
        Debug.Log("OnResetMovingArrows");

        if (in3dMode)
        {
            movArrows.UpdateMovementRestriction(false, false, false);
            movArrows.SetBoundaries(_coulombLogic.minBoundary3d.transform, _coulombLogic.maxBoundary3d.transform);
        }
        else
        {
            movArrows.UpdateMovementRestriction(false, false, true);
            movArrows.SetBoundaries(_coulombLogic.minBoundary2d.transform, _coulombLogic.maxBoundary2d.transform);
        }
        movArrows.gameObject.SetActive(true);
    }

    public void ResetObject()
    {
        
    }

    public void ResetWholeObject()
    {
        gameObject.SetActive(false);
    }

    public void HideObject()
    {
        gameObject.SetActive(false);
        gameObject.transform.parent = _coulombLogic.transform.parent;
    }
}
