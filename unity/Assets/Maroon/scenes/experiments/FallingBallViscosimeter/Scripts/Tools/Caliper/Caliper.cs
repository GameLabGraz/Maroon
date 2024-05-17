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
    }


}
