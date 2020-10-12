using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

public class CoulombAssessmentRotation : MonoBehaviour
{
    public enum UpdateMode
    {
        UM_Continuously,
        UM_OnFunctionCall,
        UM_OnToleranceOverrun
    }

    [SerializeField]
    public QuantityVector3 eulerRotation = new Vector3(0,0,0);
    [Tooltip("When OnFunctionCall is used, do not forget to call UpdatePosition().")]
    public UpdateMode updateMode = UpdateMode.UM_OnToleranceOverrun;
    public bool ignoreRuntime = false;
    [Range(0f, 0.01f)]
    public float tolerance = 0.01f;

    private CoulombLogic _coulombLogic;
    private Vector3 _lastRotation;
    private Transform _transform;
    
    // Start is called before the first frame update
    void Awake()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();

        _transform = transform;

        eulerRotation.Value = _transform.rotation.eulerAngles;
        eulerRotation.onNewValueFromSystem.AddListener(ChangeToNewRotation);
    }
    
    private void Update()
    {
        if (ignoreRuntime && SimulationController.Instance.SimulationRunning) 
            return;
        
        switch (updateMode)
        {
            case UpdateMode.UM_OnFunctionCall:
                return;
            case UpdateMode.UM_Continuously:
                if (isActiveAndEnabled)
                {
                    eulerRotation.Value = _transform.rotation.eulerAngles;
                }
                return;
            case UpdateMode.UM_OnToleranceOverrun:
            {
                if (isActiveAndEnabled &&
                    Vector3.Distance(eulerRotation.Value, _transform.rotation.eulerAngles) > tolerance)
                {
                    eulerRotation.Value = _transform.rotation.eulerAngles;
                } 
            } return;
        }
    }

    public void UpdateRotation()
    {
        if (isActiveAndEnabled)
        {
            eulerRotation.Value = _transform.rotation.eulerAngles;
        }
    }
    
    private void ChangeToNewRotation(Vector3 newEulerRotation)
    {
        if (!isActiveAndEnabled) return;
        _transform.rotation = Quaternion.Euler(newEulerRotation);
        UpdateRotation();
    }
}

