using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeaterButton : MonoBehaviour
{
    private OilHeatingElement _oilHeatingElement;
    private MeshRenderer _meshRenderer;
    private Material _material;

    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;
    [SerializeField] private bool isHeatingButton = true;
    [SerializeField] private HeaterButton _oppositeButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _oilHeatingElement = OilHeatingElement.Instance;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material.color = _offColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        bool isOn;
        if (isHeatingButton)
        {
            isOn = _oilHeatingElement.switchHeating();
        }
        else
        {
            isOn = _oilHeatingElement.switchCooling();
        }
        
        if (isOn)
        {
            _meshRenderer.material.color = _onColor;
            _oppositeButton.turnOff();
        }
        else
        {
            _meshRenderer.material.color = _offColor;
        }
    }

    public void turnOff()
    {
        _meshRenderer.material.color = _offColor;
    }
}
