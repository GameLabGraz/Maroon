using System;
using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(VRLinearDrive))]
public class ResetLinearDrive : MonoBehaviour, IResetObject
{
    protected VRLinearDrive _drive;

    public UnityEvent onAfterReset = new UnityEvent();

    public void Start()
    {
        _drive = GetComponent<VRLinearDrive>();
    }

    public void ResetObject()
    {
        _drive.ForceToValue(_drive.initialValue);
        onAfterReset.Invoke();
    }
}
