using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent onSystemSetToInvalidPosition;
    public UnityEvent onSystemShowObject;

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
        
        position.onNewValueFromSystem.AddListener(ChangeToNewPosition);
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
    
    public void UpdatePosition()
    {
        var isActive = gameObject ? gameObject.activeInHierarchy : isActiveAndEnabled;
        
        if(!_coulombLogic)
            Awake();
        
        position.Value = isActive ? _coulombLogic.WorldToCalcSpace(_transform) : new Vector3(-1, -1, -1);
    }

    private void ChangeToNewPosition(Vector3 newPosition)
    {
        if (!isBetween(newPosition.x) || !isBetween(newPosition.y) || !isBetween(newPosition.z))
        {
            onSystemSetToInvalidPosition.Invoke();
            return;
        }
        
        if(!isBetween(position.Value.x) || !isBetween(position.Value.y) || !isBetween(position.Value.z) || 
           !isActiveAndEnabled)
            onSystemShowObject.Invoke();

        if (_coulombLogic.IsIn2dMode())
        {
            var currentPos = _coulombLogic.xOrigin2d.position;
            currentPos.x += _coulombLogic.CalcToWorldSpace(newPosition.x); // Value is between 0 and 1
            currentPos.y += _coulombLogic.CalcToWorldSpace(newPosition.y); // Value is between 0 and 1
            currentPos.z += _coulombLogic.CalcToWorldSpace(0f); // Value is 0 -> 2d mode
            // currentPos.z += _coulombLogic.CalcToWorldSpace(newPosition.z); // Value is between 0 and 1
            transform.position = currentPos;
        }
        else
        {
            var currentPos = _coulombLogic.xOrigin3d.localPosition;
            currentPos.x += _coulombLogic.CalcToWorldSpace(newPosition.x, true); // Value is between 0 and 1
            currentPos.y += _coulombLogic.CalcToWorldSpace(newPosition.y, true); // Value is between 0 and 1
            currentPos.z += _coulombLogic.CalcToWorldSpace(newPosition.z, true); // Value is between 0 and 1
            transform.localPosition = currentPos;
        }
        
        UpdatePosition();
    }

    private bool isBetween(float value, float min = 0f, float max = 1f)
    {
        return min <= value && value <= max;
    }
}
