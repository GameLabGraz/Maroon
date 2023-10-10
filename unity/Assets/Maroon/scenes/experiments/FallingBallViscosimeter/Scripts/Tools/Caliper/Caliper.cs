using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Caliper : MonoBehaviour
{
    private Transform slider;
    private Transform head;
    private TMP_Text text;
    private float measuredLength;
    
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        slider = transform.Find("Slider");
        head = transform.Find("Head");

    }

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
        measuredLength = (head.position.x - slider.position.x + 0.0075f) * -1;
        text.text = Math.Round(measuredLength, 4).ToString("N4");
    }
}
