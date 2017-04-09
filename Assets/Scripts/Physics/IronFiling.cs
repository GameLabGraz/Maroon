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
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to create a field image with iron filing
/// </summary>
public class IronFiling : MonoBehaviour, IResetObject
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
    private float height;

    /// <summary>
    /// The filling width
    /// </summary>
    private float width;

    /// <summary>
    /// The height offset
    /// </summary>
    private float height_offset;

    /// <summary>
    /// The width offset
    /// </summary>
    private float width_offset;

    /// <summary>
    /// The maximum of vertices
    /// </summary>
    public int maxvertexCount = 100;
    public float lineSegmentLength = 0.1f;

    private SimulationController simController;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        height = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.z * gameObject.transform.localScale.y;
        width = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * gameObject.transform.localScale.x;
        height_offset = gameObject.transform.position.y;
        width_offset = gameObject.transform.position.x;

        field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>();
        linerenderers = new LineRenderer[2 * iterations];

        for (int i = 0; i < iterations * 2; i++)
        {
            GameObject line = new GameObject("line");
            line.transform.parent = this.transform;
            LineRenderer linerenderer = line.AddComponent<LineRenderer>();
            linerenderer.shadowCastingMode = ShadowCastingMode.On;
            linerenderer.receiveShadows = true;
            linerenderer.material = ironMaterial;
            linerenderer.useLightProbes = false;
            linerenderer.SetWidth(0.2f, 0.002f);
            linerenderers[i] = linerenderer;
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame and disables the object during a running simulation.
    /// </summary>
    void Update()
    {
        if (simController.SimulationRunning)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Stops the simulations and generate the field image by drawing random irons.
    /// </summary>
    public void generateFieldImage()
    {
        if (field == null)
            return;

        simController.StopSimulation();
        simController.AddNewResetObject(this);

        Debug.Log("Start IronFiling");

        for (int i = 0; i < iterations * 2; i++)
        {
            Vector3 origin = new Vector3(Random.Range(-1f, 1f) * 25f, Random.Range(-1f, 1f) * 25f, 0);
            drawIron(origin, linerenderers[i]);
        }
        Debug.Log("End IronFiling");

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Draws a iron line starting at the given origin.
    /// </summary>
    /// <param name="origin">The origin to start</param>
    /// <param name="linerender">The line renderer to draw line</param>
    private void drawIron(Vector3 origin, LineRenderer linerender)
    {
        List<Vector3> linePoints = new List<Vector3>();
        linePoints.Add(new Vector3(origin.x, origin.y, origin.z));

        Vector3 newLinePoint = origin;
        int numberOfPoints = 1;
        while (newLinePoint.x <= width_offset + width / 2.0f && newLinePoint.x >= width_offset - width / 2.0f &&
               newLinePoint.y <= height_offset + height / 2.0f && newLinePoint.y >= height_offset - height / 2.0f && numberOfPoints < maxvertexCount)
        {
            newLinePoint += Vector3.Normalize(field.get(newLinePoint)) * lineSegmentLength;
            linePoints.Add(new Vector3(newLinePoint.x, newLinePoint.y, newLinePoint.z));
            numberOfPoints++;
        }

        linerender.SetVertexCount(numberOfPoints);
        for (int i = 0; i < numberOfPoints; i++)
        {
            linerender.SetPosition(i, linePoints[i]);
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void resetObject()
    {
        gameObject.SetActive(false);
    }
}
