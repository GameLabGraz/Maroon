using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeasuringTape : MonoBehaviour
{
    
    [SerializeField]private Transform slider;
    [SerializeField]private Transform head;
    [SerializeField]private TMP_Text text;
    [SerializeField]private float measuredLength;
    [SerializeField] private float offset;
    // Update is called once per frame
    void Update()
    {
        updateText();
    }

    void updateText()
    {
        //sets the text to the measured distance
        Debug.Log("Slider Position: " + slider.position.x);
        Debug.Log("Head Position: " + head.position.x);
        measuredLength = (slider.position.x - head.position.x + offset) * -100;
        text.text = Math.Round(measuredLength, 2).ToString("N2") + "cm";
        Debug.Log(measuredLength);
    }
}
