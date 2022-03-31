//-----------------------------------------------------------------------------
// IronFiling.cs
//
// Class to create a field image with iron filing
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Class to create a field image with iron filing
/// </summary>
/// TODO: PLEASE RENAME TO IronFiLLing -> with 2 L ;)
public class scrIronFilings : MonoBehaviour, IResetObject
{
    /// <summary>
    /// The iron material
    /// </summary>
    public Material ironMaterial;

    /// <summary>
    /// Tha number of iteration
    /// </summary>
    public int iterations = 1000;

    /// <summary>
    /// The field which is represented
    /// </summary>
    private IField field;

    /// <summary>
    /// The line renderers to draw the field
    /// </summary>
    private LineRenderer[] linerenderers;

    /// <summary>
    /// The filling height
    /// </summary>
    public float height;

    /// <summary>
    /// The filling width
    /// </summary>
    public float width;
    //TODO: remove once width and height are correctly received
    public bool usePredefinedSize = false;

    /// <summary>
    /// The maximum of vertices
    /// </summary>
    public int maxvertexCount = 100;

    [SerializeField]
    private float lineSegmentLength = 0.1f;

    [SerializeField]
    private float lineStartWidth = 0.4f;

    [SerializeField]
    private float lineEndWidth = 0.004f;

    [SerializeField] private UnityEvent onHideIronFilings;

    private List<GameObject> _lines = new List<GameObject>();
    private bool _linesVisible = false;
    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        if (!usePredefinedSize)
        {
            height = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.z;
            width = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x;
            // Debug.Log("Iron Filling: " + height + " x " + width);
        }

        field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>();
        linerenderers = new LineRenderer[2 * iterations];

        for (var i = 0; i < iterations * 2; i++)
        {
            var line = new GameObject("line");
            line.transform.parent = this.transform;
            line.SetActive(true);
            _lines.Add(line);
            //line.transform.localRotation = Quaternion.identity;

            var linerenderer = line.AddComponent<LineRenderer>();
            //linerenderer.useWorldSpace = false;
            linerenderer.shadowCastingMode = ShadowCastingMode.On;
            linerenderer.receiveShadows = true;
            linerenderer.material = ironMaterial;
            linerenderer.lightProbeUsage = LightProbeUsage.Off;
            linerenderer.startWidth = lineStartWidth;
            linerenderer.endWidth = lineEndWidth;
            linerenderers[i] = linerenderer;
        }
        gameObject.SetActive(false);
        onHideIronFilings.Invoke();
    }

    /// <summary>
    /// Update is called every frame and disables the object during a running simulation.
    /// </summary>
    private void Update()
    {
        if (SimulationController.Instance.SimulationRunning)
        {
            gameObject.SetActive(false);
            onHideIronFilings.Invoke();
        }
    }

    /// <summary>
    /// Stops the simulations and generate the field image by drawing random irons.
    /// </summary>
    public void generateFieldImage()
    {
        if (field == null || _linesVisible)
            return;

        SimulationController.Instance.StopSimulation();

        Debug.Log("Start IronFiling");
        
        foreach( var child in _lines)
        {
            child.SetActive(true);
        }

        for (var i = 0; i < iterations * 2; i++)
        {
            var origin = new Vector3(Random.Range(-1f, 1f) * width/2, 0, Random.Range(-1f, 1f) * height/2);
            drawIron(origin, linerenderers[i]);
        }
        Debug.Log("End IronFiling");

        gameObject.SetActive(true);
        _linesVisible = true;
    }

    public void hideFieldImage()
    {
        if (!_linesVisible) return;
        
        gameObject.SetActive(false);
        
        // foreach (var child in _lines)
        // {
        //     if (child) child.SetActive(false);
        // }

        _linesVisible = false;
        onHideIronFilings.Invoke();
    }

    /// <summary>
    /// Draws a iron line starting at the given origin.
    /// </summary>
    /// <param name="origin">The origin to start</param>
    /// <param name="linerender">The line renderer to draw line</param>
    private void drawIron(Vector3 origin, LineRenderer linerender)
    {
        // line renderer points in world space!
        var linePoints = new List<Vector3> { transform.TransformPoint(origin) };

        var linePoint = origin;
        var numberOfPoints = 1;
        while (linePoint.x <= width / 2.0f && linePoint.x >= -width / 2.0f &&
               linePoint.z <= height / 2.0f && linePoint.z >= -height / 2.0f && numberOfPoints < maxvertexCount)
        {
            var globalLinePoint = transform.TransformPoint(linePoint);
            globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentLength;

            linePoints.Add(globalLinePoint);

            linePoint = transform.InverseTransformPoint(globalLinePoint);

            numberOfPoints++;
        }

        linerender.positionCount = numberOfPoints;
        for (var i = 0; i < numberOfPoints; i++)
        {
            linerender.SetPosition(i, linePoints[i]);
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        gameObject.SetActive(false);
        onHideIronFilings.Invoke();
    }
}
