﻿using System;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class PC_Rotation : MonoBehaviour, IResetWholeObject
{
    [Tooltip("The speed with which the object rotates.")]
    public float rotationSpeed = 20;
    [Tooltip("The object that should be rotated. If null then the object with the script on it will be rotated.")]
    public GameObject rotationObject = null;

    [Header("Other Settings")] 
    [Tooltip("Tells if zoom via + and - buttons is enabled.")]
    public bool enableShortcuts = true;
    [Tooltip("Stops the simulation while rotating when enabled.")]
    public bool onlyRotateOnSimulationPause;

    public UnityEvent OnStartRotation;
    public UnityEvent OnRotate;
    public UnityEvent OnEndRotation;
    
    private Transform rotateTrans;
    
    private void Start()
    {
        rotateTrans = rotationObject == null ? transform : rotationObject.transform;
    }

    private void Update()
    {
        if (!enableShortcuts) return;
        var rotated = true;
        
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            rotateTrans.RotateAround(Vector3.up, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            rotateTrans.RotateAround(Vector3.up, -rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            rotateTrans.RotateAround(Vector3.right, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            rotateTrans.RotateAround(Vector3.right, -rotationSpeed * Mathf.Deg2Rad * Time.deltaTime);
        else
            rotated = false;
        
        if(rotated)
            OnRotate.Invoke();
    }

    private void OnMouseDown()
    {
        OnStartRotation.Invoke();
    }

    private void OnMouseDrag()
    {
        if (!enabled) return;
        
        // taken from https://www.youtube.com/watch?v=S3pjBQObC90
        if(onlyRotateOnSimulationPause) SimulationController.Instance.SimulationRunning = false;
        
        var rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
        var rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;
        
        rotateTrans.Rotate(Vector3.up, -rotX);
        rotateTrans.Rotate(Vector3.right, rotY);
        
        OnRotate.Invoke();
    }

    private void OnMouseUp()
    {
        OnEndRotation.Invoke();
    }

    public void ResetObject()
    {
    }

    public void ResetWholeObject()
    {
        if(rotateTrans == null) 
            rotateTrans = rotationObject == null ? transform : rotationObject.transform;
        
        rotateTrans.rotation = Quaternion.identity;
    }
}
