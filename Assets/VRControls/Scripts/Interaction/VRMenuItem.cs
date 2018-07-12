using System;
using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

public class VRMenuItem : MonoBehaviour
{
    public VRInteractiveItem interactiveItem;
    public bool animatedHover = true;
    public bool popOutOnHover = true;
    public bool increaseSizeOnHover = true;
    public bool doubleClick = false;

    [HideInInspector]
    public bool isClicked = false;

    Vector3 startPosition, startScale;
    Vector3 targetPosition, targetScale;
    
	protected virtual void Start () {
	    interactiveItem.OnOver += onOver;
        interactiveItem.OnOut += onOut;
        interactiveItem.OnClick += onClick;
        interactiveItem.OnDoubleClick += onDoubleClick;

	    startPosition = transform.position;
	    startScale = transform.localScale;
	}

    void onDoubleClick()
    {
        if (doubleClick)
            handleCorrectClick();
    }

    void onClick()
    {
        if (!doubleClick)
            handleCorrectClick();
    }

    void handleCorrectClick()
    {
        isClicked = !isClicked;
        if (isClicked)
            onActivate();
        else
        {
            onDeactivate();
        }
    }

    void onOut()
    {
        if (!animatedHover)
            return;

        targetPosition = startPosition;
        targetScale = startScale;
        StopCoroutine("hoverAnimationRoutine");
        StartCoroutine("hoverAnimationRoutine");
    }

    void onOver()
    {
        if (!animatedHover)
            return;

        float distance = (Camera.main.transform.position - startPosition).magnitude;
        targetPosition = Vector3.Lerp(startPosition, Camera.main.transform.position, .1f/distance);
        targetScale = startScale*1.1f;
        StopCoroutine("hoverAnimationRoutine");
        StartCoroutine("hoverAnimationRoutine");
    }

    IEnumerator hoverAnimationRoutine()
    {
        while (popOutOnHover && (transform.position - targetPosition).sqrMagnitude > 0.001f
               || increaseSizeOnHover && (transform.localScale - targetScale).sqrMagnitude > 0.001f)
        {
            if (popOutOnHover)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * .8f);
                transform.position = pos;
            }
            if (increaseSizeOnHover)
            {
                Vector3 scale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * .7f * startScale.x);
                transform.localScale = scale;
            }
            yield return null;
        }
    }

    protected virtual void onActivate()
    {
        Debug.Log("Activated");
    }

    protected virtual void onDeactivate()
    {
        Debug.Log("Deactivated");
    }
}
