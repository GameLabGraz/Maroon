using System;
using UnityEngine;
using VRTK.UnityEventHelper;

public class DisableComponentOnEvent : MonoBehaviour
{
    private VRTK_InteractableObject_UnityEvents _events;

    public enum InteractableObjectEvent
    {
        OnGrab,
        OnUngrab,
        OnTouch,
        OnUntouch,
        OnUse,
        OnUnuse,
        OnEnterSnapDropZone,
        OnExitSnapDropZone,
        OnSnapToDropZone,
        OnUnsnapFromDropZone
    }

    public string ComponentName;
    public InteractableObjectEvent EnableEvent = InteractableObjectEvent.OnGrab;
    public InteractableObjectEvent DisableEvent = InteractableObjectEvent.OnGrab;

    private void Start()
    {
        VRTK_InteractableObject_UnityEvents.InteractableObjectEvent enableEvent =
            (VRTK_InteractableObject_UnityEvents.InteractableObjectEvent) _events.GetType().GetField(EnableEvent.ToString()).GetValue(_events);
        enableEvent.AddListener((o, e) => Handle(true));
        
        VRTK_InteractableObject_UnityEvents.InteractableObjectEvent disableEvent =
            (VRTK_InteractableObject_UnityEvents.InteractableObjectEvent) _events.GetType().GetField(DisableEvent.ToString()).GetValue(_events);
        disableEvent.AddListener((o, e) => Handle(false));
    }

    private void Awake()
    {
        _events = GetComponent<VRTK_InteractableObject_UnityEvents>();
    }

    private void Handle(bool enable)
    {
        Type t = Type.GetType(ComponentName);
        var comp = GetComponent(t);
        Behaviour be = comp as Behaviour;
        if (be != null)
            be.enabled = enable;
        else
        {
            Renderer rend = comp as Renderer;
            if (rend != null)
                rend.enabled = enable;
        }
    }
}