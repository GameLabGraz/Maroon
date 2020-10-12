using System;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class VoltmeterEvent : UnityEvent<string> {}

public class VoltmeterMeasuringPoint : MonoBehaviour, IResetWholeObject
{
    public IField field;
    public VoltmeterEvent onVoltageChanged;
    public VoltmeterEvent onVoltageChangedUnit;

    public QuantityVector3 calculatedPosition;

    private CoulombLogic _coulombLogic;
    private float _potential;
    
    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
        onVoltageChanged.Invoke("---");
    }

    // Update is called once per frame
    void Update()
    {
        var currentPos = transform.position;
        if (!gameObject.activeInHierarchy || !field) return;

        _potential = field.getStrength(currentPos); //in V
        onVoltageChanged.Invoke(GetCurrentFormattedString());
        onVoltageChangedUnit.Invoke(GetCurrentUnit());
    }

    public string GetCurrentUnit()
    {
        var checkPotential = _potential;
        if (checkPotential > 1f)
            return "V";
        checkPotential *= Mathf.Pow(10, 3);
        return checkPotential > 1f ? "mV" : "\u00B5V";
    }

    public string GetCurrentFormattedString()
    {
        var checkPotential = _potential;
        for (var cnt = 0; checkPotential < 1f && cnt < 2; ++cnt)
        {
            checkPotential *= Mathf.Pow(10, 3);
        }
            
        return checkPotential.ToString("F");   
    }
    
    public float GetPotentialInMicroVolt()
    {
        return _potential; 
    }
    
    public void OnResetMovingArrows(bool in3dMode)
    {
        if (!_coulombLogic)
        {
            var simControllerObject = GameObject.Find("CoulombLogic");
            if (simControllerObject)
                _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        }
        var dragHandler = GetComponent<PC_DragHandler>();
        if (dragHandler)
        {
            if (in3dMode)
            {
                gameObject.transform.parent = _coulombLogic.scene3D.transform;
                dragHandler.SetBoundaries(_coulombLogic.minBoundary3d.gameObject,
                    _coulombLogic.maxBoundary3d.gameObject);
                dragHandler.allowedXMovement = dragHandler.allowedYMovement = dragHandler.allowedZMovement = true;
            }
            else
            {
                gameObject.transform.parent = _coulombLogic.scene2D.transform;
                dragHandler.SetBoundaries(_coulombLogic.minBoundary2d.gameObject,
                    _coulombLogic.maxBoundary2d.gameObject);
                dragHandler.allowedXMovement = dragHandler.allowedYMovement = true;
                dragHandler.allowedZMovement = false;
            }
        }

        var movArrows = GetComponentInChildren<PC_ArrowMovement>();
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
    
    public void ResetObject()
    {
        
    }

    public void ResetWholeObject()
    {
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.SetActive(false);
    }

    public void HideObject()
    {
        gameObject.SetActive(false);
        if(_coulombLogic)
            gameObject.transform.parent = _coulombLogic.transform.parent;
        onVoltageChanged.Invoke("---");
    }

    public void ShowObject()
    {
        Debug.Assert(_coulombLogic != null);
        gameObject.SetActive(true);
        transform.parent = _coulombLogic.IsIn2dMode()
            ? _coulombLogic.scene2D.transform
            : _coulombLogic.scene3D.transform;
        transform.localRotation = Quaternion.identity;
    }
}
