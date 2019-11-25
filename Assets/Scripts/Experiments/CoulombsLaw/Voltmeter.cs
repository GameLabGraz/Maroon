using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class VoltmeterEvent : UnityEvent<string> {}

public class Voltmeter : MonoBehaviour, IResetWholeObject
{
    public IField field;
    public VoltmeterEvent onVoltageChanged;
    public VoltmeterEvent onVoltageChangedUnit;
    
    private CoulombLogic _coulombLogic;
    private float _potential;
    
    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);

//        // There is only one voltmeter allowed.
//        FindObjectsOfType<Voltmeter>()
//            .Where(voltmeter => voltmeter != this)
//            .ForEach(voltmeter => Destroy(voltmeter.gameObject));

        onVoltageChanged.Invoke("---");
    }

    // Update is called once per frame
    void Update()
    {
        var currentPos = transform.position;
        if (!gameObject.activeInHierarchy) return;

        _potential = field.getStrength(currentPos) * Mathf.Pow(10, -6); //in V now (because we used microCoulomb)
        var pot = _potential; //in micro Volt
        var potentialString = pot.ToString("F"); 
//            var potentialString = potential.ToString("0.000") + " * 10^-5 V";
//            Debug.Log("OnVoltageChanged: " + potentialString);            
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
        if (in3dMode)
        {
            gameObject.transform.parent = _coulombLogic.scene3D.transform;
            dragHandler.SetBoundaries(_coulombLogic.minBoundary3d.gameObject, _coulombLogic.maxBoundary3d.gameObject);
            dragHandler.allowedXMovement = dragHandler.allowedYMovement = dragHandler.allowedZMovement = true;
        }
        else
        {
            gameObject.transform.parent = _coulombLogic.scene2D.transform;
            dragHandler.SetBoundaries(_coulombLogic.minBoundary2d.gameObject, _coulombLogic.maxBoundary2d.gameObject);
            dragHandler.allowedXMovement = dragHandler.allowedYMovement = true;
            dragHandler.allowedZMovement = false;
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
}
