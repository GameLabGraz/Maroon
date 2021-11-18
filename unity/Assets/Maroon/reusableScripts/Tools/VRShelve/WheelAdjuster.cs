using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAdjuster : MonoBehaviour
{
    [SerializeField] private List<Transform> wheels = new List<Transform>();
    [SerializeField] private float distanceOffset = 0.01f;

    private Vector3 _position;
    // Start is called before the first frame update
    void Start()
    {
        _position = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position ,_position) < distanceOffset) return;
        
        var currentPos = transform.position;
        var rad = Mathf.Atan2(currentPos.z - _position.z, currentPos.x -_position.x);
        var degree = (180f / Mathf.PI) * rad;
            
        foreach (var wheel in wheels)
        {
            wheel.rotation = Quaternion.Euler(0, degree, 90f);
        }
        _position = currentPos;
    }
}
