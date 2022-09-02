using System;
using GameLabGraz.VRInteraction;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LockedDrawer : MonoBehaviour
{
    public VRHighlighter highlighter = null;
    public GameObject lockIcon = null;
    public Color highlightColor = new Color(0.85f, 0.6f, 0.15f);

    protected void Start()
    {
        if(lockIcon)
            lockIcon.SetActive(false);
    }
    
    protected virtual void OnHandHoverBegin(Hand hand)
    {
        if (!isActiveAndEnabled) return;
        ShowLockIcon(true);
    }

    protected virtual void OnHandHoverEnd(Hand hand)
    {
        if (!isActiveAndEnabled) return;
        ShowLockIcon(false);
    }

    protected void OnEnable()
    {
        var interactable = GetComponent<VRInteractable>();
        if (interactable && interactable.hoverable && interactable.isHovering)
        {
            ShowLockIcon(true);
        }
    }

    protected void OnDisable()
    {
        ShowLockIcon(false);
    }

    public void ShowLockIcon(bool show)
    {
        if(lockIcon)
            lockIcon.SetActive(show);

        if (highlighter)
        {
            if(show) highlighter.Highlight(highlightColor);
            else highlighter.ResetToDefault();
        }
    }
}
