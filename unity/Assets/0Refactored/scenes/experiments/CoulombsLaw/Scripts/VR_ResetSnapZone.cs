using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

public class VR_ResetSnapZone : VRTK_SnapDropZone, IResetWholeObject
{
    public bool resetScaleOnSnap;
    private Vector3 globalScale;
    private bool scaleSet = false;
    private bool scaleUpdated = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!scaleSet)
        {
            var currentTransform = defaultSnappedInteractableObject.transform;
            globalScale = currentTransform.localScale;
            Debug.Log("Global Scale: " + globalScale);
            scaleSet = true;
        }
    }

    public override void OnObjectSnappedToDropZone(SnapDropZoneEventArgs e)
    {
        base.OnObjectSnappedToDropZone(e);
        ResetScale();
    }

    public void ResetScale()
    {
        
        if (resetScaleOnSnap && currentSnappedObject && scaleSet)
            currentSnappedObject.transform.localScale = globalScale;
    }
    
    public void ResetObject() {

    }

    public void ResetWholeObject()
    {
        OnEnable();
    }
}
