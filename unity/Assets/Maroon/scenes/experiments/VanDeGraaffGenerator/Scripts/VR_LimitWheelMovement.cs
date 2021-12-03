using System.Collections;
using System.Collections.Generic;
using GEAR.VRInteraction;
using UnityEngine;

[RequireComponent(typeof(VRCircularDrive))]
public class VR_LimitWheelMovement : MonoBehaviour
{
    private VRCircularDrive _circularDrive;
    // Start is called before the first frame update
    void Start()
    {
        _circularDrive = GetComponent<VRCircularDrive>();
    }

    public void LimitMinimum()
    {
        _circularDrive.minAngle = _circularDrive.outAngle;
    }

    public void LimitMaximum()
    {
        _circularDrive.maxAngle = _circularDrive.outAngle;
    }

    public void RemoveLimits()
    {
        _circularDrive.minAngle = -Mathf.Infinity;
        _circularDrive.maxAngle = Mathf.Infinity;
    }
    
}
