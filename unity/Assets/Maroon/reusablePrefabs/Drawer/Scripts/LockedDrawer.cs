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
        if(lockIcon)
            lockIcon.SetActive(true);
        
        if(highlighter)
            highlighter.Highlight(highlightColor);
    }

    protected virtual void OnHandHoverEnd(Hand hand)
    {
        if(lockIcon)
            lockIcon.SetActive(false);
        
        if(highlighter)
            highlighter.ResetToDefault();
    }
}
