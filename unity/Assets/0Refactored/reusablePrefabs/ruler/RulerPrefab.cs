using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class RulerDistanceEvent : UnityEvent<float> {}

public class RulerPrefab : MonoBehaviour, IResetWholeObject
{
    bool locked = false;
    Vector3 start_to_end;

    private bool DeactivateOnReset = true;
    
    public GameObject RulerEverything;
    public GameObject RulerStart;
    public GameObject RulerEnd;
    public LineRenderer RulerLine;

    public RulerDistanceEvent onDistanceChanged;

    private CoulombLogic _coulombLogic;
    public QuantityFloat currentDistance = 0;
    private Vector3 _startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);

        if (RulerLine.positionCount < 2)
        {
            RulerLine.SetPositions(new[]{RulerStart.transform.position, RulerEnd.transform.position});
        }

        currentDistance.Value = 0;
        onDistanceChanged.Invoke(0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (RulerStart.activeSelf && RulerEnd.activeSelf)
        {
            if(!RulerLine.gameObject.activeSelf)
                RulerLine.gameObject.SetActive(true);
            var startPosition = RulerStart.transform.position;
            var endPosition = RulerEnd.transform.position;
            RulerLine.SetPosition(0, startPosition);
            RulerLine.SetPosition(1, endPosition);
            var newDist = _coulombLogic.WorldToCalcSpace(Vector3.Distance(startPosition, endPosition));

            if (Math.Abs(newDist - currentDistance) > 0.0001f)
            {
                currentDistance.Value = newDist;
                onDistanceChanged.Invoke(currentDistance);
            }
        }
        else if(RulerLine.gameObject.activeSelf)
        { 
            RulerLine.gameObject.SetActive(false);
            currentDistance.Value = 0f;
            onDistanceChanged.Invoke(0f);
        }
    }

    public void ResetObject()
    {
        
    }

    public void ResetWholeObject()
    {
        transform.localRotation = Quaternion.identity;

        transform.position = _startPosition;

        if (DeactivateOnReset)
        {
            RulerStart.SetActive(false);
            RulerEnd.SetActive(false);
            var dragHandler = RulerStart.GetComponent<PC_DragHandler>();
            if(dragHandler) dragHandler.onDisabled.Invoke();
            dragHandler = RulerEnd.GetComponent<PC_DragHandler>();
            if(dragHandler) dragHandler.onDisabled.Invoke();
        }
    }
    
    public void OnChangeMode(bool in3dMode)
    {
        if (!_coulombLogic)
        {
            var simControllerObject = GameObject.Find("CoulombLogic");
            if (simControllerObject)
                _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        }
        if (in3dMode)
            transform.parent = _coulombLogic.scene3D.transform;
        else
            transform.parent = _coulombLogic.scene2D.transform;
       
        ResetMovingArrow(in3dMode, RulerStart);
        ResetMovingArrow(in3dMode, RulerEnd);
    }

    private void ResetMovingArrow(bool in3dMode, GameObject obj)
    {
        var dragHandler = obj.GetComponent<PC_DragHandler>();
        if (in3dMode)
        {
            dragHandler.SetBoundaries(_coulombLogic.minBoundary3d.gameObject, _coulombLogic.maxBoundary3d.gameObject);
            dragHandler.allowedXMovement = dragHandler.allowedYMovement = dragHandler.allowedZMovement = true;
        }
        else
        {
            dragHandler.SetBoundaries(_coulombLogic.minBoundary2d.gameObject, _coulombLogic.maxBoundary2d.gameObject);
            dragHandler.allowedXMovement = dragHandler.allowedYMovement = true;
            dragHandler.allowedZMovement = false;     
        }

        var movArrows = obj.GetComponentInChildren<PC_ArrowMovement>();
        if (!movArrows) return;
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

    public IQuantity GetDistance()
    {
        return currentDistance;
    }
}
