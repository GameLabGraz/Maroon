using System;
using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(VRSnapDropZone))]
public class VRResetSnapZone : MonoBehaviour, IResetObject
{
    [SerializeField]
    protected Interactable resetSnapObject = null;
    private VRSnapDropZone _snapZone;
    
    protected void Start()
    {
        _snapZone = GetComponent<VRSnapDropZone>();
        Debug.Assert(_snapZone != null);
    }

    public void ResetObject()
    {
        if (!resetSnapObject.attachedToHand && !_snapZone.snappedObject)
        {
            _snapZone.Snap(resetSnapObject.gameObject);
        }
    }
}