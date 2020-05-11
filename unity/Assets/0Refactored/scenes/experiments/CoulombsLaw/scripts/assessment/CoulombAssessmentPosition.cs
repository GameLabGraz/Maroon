using System;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

public class CoulombAssessmentPosition : MonoBehaviour
{
    [SerializeField]
    public QuantityVector3 position;
    public bool observeLocal = true;
    [Range(0f, 0.1f)]
    public float tolerance = 0.00001f;

    private CoulombLogic _coulombLogic;
    private Vector3 _lastPosition;
    private Transform _transform;
    
    // Start is called before the first frame update
    void Awake()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();

        _transform = transform;
        
        position.Value = _coulombLogic.WorldToCalcSpace(_transform);
        _lastPosition = observeLocal ? _transform.localPosition : _transform.position;
    }
    
    private void Update()
    {
        if (_transform.gameObject.activeSelf && Vector3.Distance(_lastPosition, observeLocal ? _transform.localPosition : _transform.position) > tolerance)
        {
            position.Value = _coulombLogic.WorldToCalcSpace(_transform);
            _lastPosition = position.Value;
        }
        else if (!_transform.gameObject.activeSelf && position.Value.x >= 0)
        {
            position.Value = new Vector3(-1, -1, -1);
        }
    }
    
    public IQuantity GetPosition()
    {
        return position;
    }
}
