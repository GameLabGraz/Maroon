using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


namespace Maroon.Physics.Viscosimeter
{
    public class MeasuringTape : MonoBehaviour
    {
        [SerializeField] private bool horizontal = true;
        [SerializeField] private Transform slider;
        [SerializeField] private Transform head;
        [SerializeField] private TMP_Text text;
        [SerializeField] private float measuredLength;
        [SerializeField] private float tape_end_offset;

        [SerializeField] private LineRenderer lineRenderer;

        [SerializeField] private Transform lineRendererStart;
        private Vector3[] positions = new Vector3[2];
        [SerializeField] private DragDrop hook_dnd;
        private float m_to_cm = -100;

        private void Start()
        {
            lineRenderer.positionCount = 2;
            if (transform.eulerAngles.z != 0f)
            {
                horizontal = false;
                tape_end_offset = -0.0375f;
                m_to_cm = 100;
            }
            else
            {
                tape_end_offset = 0.0375f;
                hook_dnd.axisLockedInto = Axis.Y;
            }
        }

        private void Update()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            positions[0] = lineRendererStart.position;
            positions[1] = slider.position;
            lineRenderer.SetPositions(positions);

            //sets the text to the measured distance
            if (horizontal)
            {
                measuredLength = (slider.position.x - head.position.x + tape_end_offset) * m_to_cm;
            }
            else
            {
                measuredLength = (slider.position.y - head.position.y + tape_end_offset) * m_to_cm;
            }

            text.text = Math.Round(measuredLength, 2).ToString("N2") + "cm";
        }
    }
}