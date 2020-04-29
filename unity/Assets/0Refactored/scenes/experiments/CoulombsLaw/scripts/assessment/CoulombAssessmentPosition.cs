using System;
using Maroon.Physics;
using UnityEngine;

public class CoulombAssessmentPosition : MonoBehaviour
{
    public Transform observingObject = null;
    
    [SerializeField]
    public QuantityVector3 position;
    public bool observeLocal = true;
    [Range(0f, 0.1f)]
    public float tolerance = 0.00001f;

    private CoulombLogic _coulombLogic;
    private Vector3 _lastPosition;
    private Transform _transform;
    
    // Start is called before the first frame update
    void Start()
    {
        var obj  = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();

        if (observingObject)
            _transform = observingObject;
        else
            _transform = transform;

        position = _coulombLogic.WorldToCalcSpace(_transform);
        _lastPosition = observeLocal ? _transform.localPosition : _transform.position;
    }
    
    private void Update()
    {
        if (Vector3.Distance(_lastPosition, observeLocal ? _transform.localPosition : _transform.position) > tolerance)
        {
            position = _coulombLogic.WorldToCalcSpace(_transform);
        }
    }

}
