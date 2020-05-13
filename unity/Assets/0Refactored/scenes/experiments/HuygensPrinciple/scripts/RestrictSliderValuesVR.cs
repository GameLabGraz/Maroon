using System.Collections;
using System.Collections.Generic;
using Maroon.Physics.HuygensPrinciple;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class RestrictSliderValuesVR : MonoBehaviour
{
    private GameObject plate;
    private float plateWidth;

    public GameObject sliderToRestrictVR;

    void Start()
    {
        plate = GameObject.Find("Plate");
        var plateTop = plate.transform.Find("Top");
        if(plateTop != null)
            plateWidth = plateTop.transform.localScale.x;
        else
            Debug.LogError("SlitPlate::Start: Top object cannot be null.");

        if (sliderToRestrictVR == null)
            Debug.LogError("RestrictSliderValues:_VR: Could not find slider object preSliderControlPanel, Slider.");
    }

    void Update()
    {
        
    }

    public void RestrictSlidersWidth_VR()
    {
        var numberOfSlits = gameObject.GetComponent<SliderController>().GetValue();
        var maxSlitWidth = (plateWidth / numberOfSlits) - 0.04f;

        maxSlitWidth = maxSlitWidth > 1.0f ? 1.0f : maxSlitWidth;
        sliderToRestrictVR.GetComponent<SliderController>().maximumValue = maxSlitWidth;
    }
}
