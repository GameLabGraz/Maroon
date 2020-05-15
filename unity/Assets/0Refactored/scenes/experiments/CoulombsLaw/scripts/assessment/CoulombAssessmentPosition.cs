using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

public class CoulombAssessmentPosition : MonoBehaviour
{
    public enum UpdateMode
    {
        UM_Continuously,
        UM_OnFunctionCall,
        UM_OnToleranceOverrun
    }

    [SerializeField]
    public QuantityVector3 position = new Vector3(-1, -1, -1);
    public bool observeLocal = true;
    [Tooltip("When OnFunctionCall is used, do not forget to call UpdatePosition().")]
    public UpdateMode updateMode = UpdateMode.UM_OnToleranceOverrun;
    public bool ignoreRuntime = false;
    [Range(0f, 0.01f)]
    public float tolerance = 0.1f;

    private CoulombLogic _coulombLogic;
    private Vector3 _lastPosition;
    private Transform _transform;
    private bool _lastVisible = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();

        _transform = transform;
        
        position.Value = _coulombLogic.WorldToCalcSpace(_transform);
        _lastPosition = observeLocal ? _transform.localPosition : _transform.position;
        _lastVisible = isActiveAndEnabled;
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
                position.Value = isActiveAndEnabled
                    ? _coulombLogic.WorldToCalcSpace(_transform)
                    : new Vector3(-1, -1, -1);
                return;
            case UpdateMode.UM_OnToleranceOverrun:
            {
                if (!isActiveAndEnabled)
                {
                    if (_lastVisible)
                    {
                        position.Value = new Vector3(-1, -1, -1);
                        _lastVisible = false;
                    }
                }
                else if (!_lastVisible || Vector3.Distance(_lastPosition, observeLocal ? _transform.localPosition : _transform.position) > tolerance)
                {
                    position.Value = _coulombLogic.WorldToCalcSpace(_transform);
                    _lastPosition = observeLocal? _transform.localPosition : _transform.position;
                    _lastVisible = true;
                }
            } return;
        }
    }
    
    public IQuantity GetPosition()
    {
        return position;
    }

    public void UpdatePosition()
    {
        position.Value = isActiveAndEnabled ? _coulombLogic.WorldToCalcSpace(_transform) : new Vector3(-1, -1, -1);
    }
}
