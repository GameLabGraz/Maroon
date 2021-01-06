using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkInfoPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI infoText;

    [SerializeField] private RectTransform panelRectTransform;

    [SerializeField] private float extendedHeight;
    
    [SerializeField] private float stepSize;
    
    private bool _extend;
    private float _currentHeight;
    private float _startHeight;

    private void Start()
    {
        _startHeight = panelRectTransform.sizeDelta.y;
        _currentHeight = _startHeight;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoText.text = "<color=#800000>" +
                        MaroonNetworkManager.Instance.PlayerName
                        + "</color> : <color=#800000>" +
                        MaroonNetworkManager.Instance.ServerName;
        _extend = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _extend = false;
    }

    private void FixedUpdate()
    {
        if (_extend)
        {
            _currentHeight += stepSize;
            if (_currentHeight >= extendedHeight)
            {
                _currentHeight = extendedHeight;
                infoText.gameObject.SetActive(true);
            }
        }
        else
        {
            infoText.gameObject.SetActive(false);
            _currentHeight -= stepSize;
            if (_currentHeight <= _startHeight)
            {
                _currentHeight = _startHeight;
            }
        }
        
        panelRectTransform.sizeDelta = new Vector2(panelRectTransform.sizeDelta.x, _currentHeight);
    }
}

