using System.Collections;
using System.Collections.Generic;
using Maroon.Physics.HuygensPrinciple;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class RestrictSliderValuesPC : MonoBehaviour
{
    private GameObject plate;
    private float plateWidth;

    private GameObject slitWidthPC;

    void Start()
    {
        plate = GameObject.Find("Plate");
        var plateTop = plate.transform.Find("Top");
        if(plateTop != null)
            plateWidth = plateTop.transform.localScale.x;
        else
            Debug.LogError("SlitPlate::Start: Top object cannot be null.");

        slitWidthPC = GameObject.Find("SliderGroupPlateSlitWidth").transform.Find("Slider").gameObject;
        if (slitWidthPC == null)
            Debug.LogError("RestrictSliderValues:_PC: Could not find slider object SliderGroupPlateSlitWidth, Slider.");
    }

    void Update()
    {
        
    }
    public void RestrictSlidersWidth_PC()
    {
        var numberOfSlits = plate.GetComponent<SlitPlate>().NumberOfSlits;
        var maxSlitWidth = (plateWidth / numberOfSlits) - 0.02f;
        slitWidthPC.GetComponent<PC_Slider>().maxValue = maxSlitWidth;
    }
}
