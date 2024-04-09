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
    private float measuredLength;
    [SerializeField] private float offset;
    // Update is called once per frame
    void FixedUpdate()
    {
        updateText();
    }

    void updateText()
    {
        //sets the text to the measured distance
        //floats are dumb so we get some rounding errors but whatever
        //0.0075 is the distance between the slider transform and the head transform
        measuredLength = (head.position.y - slider.position.y + offset) * -1;
        text.text = Math.Round(measuredLength, 4).ToString("N4");
    }
}
