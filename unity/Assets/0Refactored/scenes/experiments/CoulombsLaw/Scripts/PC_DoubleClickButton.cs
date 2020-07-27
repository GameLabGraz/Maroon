using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PC_DoubleClickButton : MonoBehaviour, IPointerClickHandler
{
    public float clickTime = 0.35f;
    
    public UnityEvent onClick;
    public UnityEvent onDoubleClick;
    
    private bool _clickedOnce = false;
    private float _time = 0f;

    private void Start()
    {
    }

    private void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount == 1)
            onClick.Invoke();
        else if(eventData.clickCount > 1)
            onDoubleClick.Invoke();
    }
}
