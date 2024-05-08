using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using TMPro;
using UnityEngine;

public class MeasuringTape : MonoBehaviour
{
    [SerializeField] private bool horizontal = true;
    [SerializeField]private Transform slider;
    [SerializeField]private Transform head;
    [SerializeField]private TMP_Text text;
    [SerializeField]private float measuredLength;
    [SerializeField] private float offset;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform lineRendererStart;
    private Vector3[] positions;
    [SerializeField]private DragDrop hook_dnd;
    private float m_to_cm = -100;

    private void Start()
    {
        lineRenderer.positionCount = 2;
        positions = new Vector3[2];
        if (transform.rotation.z != 0f)
        {
            horizontal = false;
            offset = -0.03745f;
            m_to_cm = 100;
        }
        else
        {
            offset = 0.0375f;
            hook_dnd.axisLockedInto = Axis.Y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateText();
    }

    void updateText()
    {
        positions[0] = lineRendererStart.position;
        positions[1] = slider.position;
        lineRenderer.SetPositions(positions);
        
        //sets the text to the measured distance
        if (horizontal)
        {
            measuredLength = (slider.position.x - head.position.x + offset) * m_to_cm;
        }
        else
        {
            measuredLength = (slider.position.y - head.position.y + offset) * m_to_cm;
        }
        text.text = Math.Round(measuredLength, 2).ToString("N2") + "cm";
    }
}
