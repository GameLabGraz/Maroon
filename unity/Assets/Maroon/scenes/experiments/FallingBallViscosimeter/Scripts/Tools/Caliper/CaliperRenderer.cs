using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaliperRenderer : MonoBehaviour
{

    public Vector3 origin;
    public float scale_total_length = 0.2f;
    public float vernier_scale_length = 0.039f;
    public int number_of_divisions = 10;
    public int vernier_subdivisions = 2;
    private decimal vernier_scale_division;
    private decimal vernier_scale_subdivision;
    private bool drawSubdivisions = false;
    
    private List<GameObject> millimeter_lines;

    public float centimeterLineHeight = 0.004f;
    public float halfCentimeterLineHeight = 0.003f;
    public float millimeterLineHeight = 0.002f;
    public float lineWidth = 0.0001f;
    public float lineOffsetZ = -0.003f;
    
    private Transform head;
    private Transform slider;
    
    // Start is called before the first frame update
    void Awake()
    {
        head = transform.Find("Head");
        slider = transform.Find("Slider");
        origin = head.position + new Vector3(0.0075f, 0.0f, 0.0f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        calculateVernierScale();
        drawMillimeterLines();
        drawVernierScale();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void calculateVernierScale()
    {
        vernier_scale_division = (decimal)vernier_scale_length / number_of_divisions;
        if (vernier_subdivisions > 1)
        {
            vernier_scale_subdivision = vernier_scale_division / vernier_subdivisions;
            drawSubdivisions = true;
        }
    }

    private void drawMillimeterLines()
    {
        int total_millimeters = (int)(Mathf.Ceil(scale_total_length / 0.001f));
        Debug.Log("Total Millimeters: " + total_millimeters);
        GameObject mm_lines = new GameObject("MMLines");
        mm_lines.transform.SetParent(gameObject.transform);
        mm_lines.transform.position = origin;
        
        int current_cm = 0;
        for (int i = 0; i <= total_millimeters; i++)
        {
            GameObject new_line = new GameObject("Line");
            new_line.transform.SetParent(mm_lines.transform);
            new_line.transform.localPosition = new Vector3(0.001f * i, 0f, lineOffsetZ);
            bool full_centimeter = (i % 10 == 0);
            bool half_centimeter = (i % 5 == 0);
            current_cm = Mathf.FloorToInt(i/10.0f);

            addMillimeterLineRenderer(new_line, full_centimeter, half_centimeter, current_cm);
            
        }
    }

    private void addMillimeterLineRenderer(GameObject lineObject, bool fullCentimeter, bool halfCentimeter, int current_cm)
    {
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        List<Vector3> positions = new List<Vector3>();
        positions.Add(Vector3.zero);
        
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.useWorldSpace = false;
        
        if (fullCentimeter)
        {
            positions.Add(new Vector3(0f, centimeterLineHeight, 0f));
            lineRenderer.SetPositions(positions.ToArray());
        }
        else if (halfCentimeter)
        {
            positions.Add(new Vector3(0f, halfCentimeterLineHeight, 0f));
            lineRenderer.SetPositions(positions.ToArray());
        }
        else
        {
            positions.Add(new Vector3(0f, millimeterLineHeight, 0f));
            lineRenderer.SetPositions(positions.ToArray());
        }
        
    }

    private void drawVernierScale()
    {
        
    }
}
