using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class RulerDistanceEvent : UnityEvent<float> {}

public class RulerPrefab : MonoBehaviour, IResetWholeObject
{
    bool locked = false;
    Vector3 start_to_end;

    public GameObject RulerEverything;
    public GameObject RulerStart;
    public GameObject RulerEnd;
    public LineRenderer RulerLine;

    public RulerDistanceEvent onDistanceChanged;

    private CoulombLogic _coulombLogic;
    private float _oldDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        RulerEverything = transform.Find("RulerEverything").gameObject;
        RulerStart = RulerEverything.transform.Find("RulerStart").gameObject;
        RulerEnd = RulerEverything.transform.Find("RulerEnd").gameObject;
        RulerLine = RulerEverything.transform.Find("RulerLine").gameObject.GetComponent<LineRenderer>();
        
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
        
        _oldDistance = 0;
        onDistanceChanged.Invoke(0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (RulerStart.gameObject.activeSelf && RulerEnd.gameObject.activeSelf)
        {
            if(!RulerLine.gameObject.activeSelf)
                RulerLine.gameObject.SetActive(true);
            RulerLine.SetPosition(0, RulerStart.transform.position);
            RulerLine.SetPosition(1, RulerEnd.transform.position);
            var newDist = _coulombLogic.WorldToCalcSpace(Vector3.Distance(RulerStart.transform.position, RulerEnd.transform.position));

            if (Math.Abs(newDist - _oldDistance) > 0.0001f)
            {
                _oldDistance = newDist;
                onDistanceChanged.Invoke(_oldDistance);
            }
        }
        else if(RulerLine.gameObject.activeSelf)
        { 
            RulerLine.gameObject.SetActive(false);
            _oldDistance = 0f;
            onDistanceChanged.Invoke(0f);
        }
    }

    public void ResetObject()
    {
        
    }

    public void ResetWholeObject()
    {
        RulerStart.SetActive(false);
        RulerEnd.SetActive(false);
        RulerStart.GetComponent<PC_DragHandler>().onDisabled.Invoke();
        RulerEnd.GetComponent<PC_DragHandler>().onDisabled.Invoke();
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
}
