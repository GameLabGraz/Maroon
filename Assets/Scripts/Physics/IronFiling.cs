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
    /// The maximum of vertices
    /// </summary>
    public int maxvertexCount = 100;
    private const bool usecoroutine = true;

    [SerializeField]
    private float lineSegmentLength = 0.1f;

    [SerializeField]
    private float lineStartWidth = 0.4f;

    [SerializeField]
    private float lineEndWidth = 0.004f;

    [SerializeField]
    private float tolerance = 0.25f;

    private SimulationController simController;


    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        height = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.z;
        width = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x;

        field = GameObject.FindGameObjectWithTag("Field").GetComponent<IField>();
        linerenderers = new LineRenderer[2 * iterations];

        for (int i = 0; i < iterations * 2; i++)
        {
            GameObject line = new GameObject("line");
            line.transform.parent = this.transform;
            //line.transform.localRotation = Quaternion.identity;

            LineRenderer linerenderer = line.AddComponent<LineRenderer>();
            //linerenderer.useWorldSpace = false;
            linerenderer.shadowCastingMode = ShadowCastingMode.Off;
            linerenderer.receiveShadows = false;
            linerenderer.material = ironMaterial;
            linerenderer.lightProbeUsage = LightProbeUsage.Off;
            linerenderer.startWidth = lineStartWidth;
            linerenderer.endWidth = lineEndWidth;
            linerenderer.allowOcclusionWhenDynamic = false;

            linerenderer.SetPosition(0, transform.TransformPoint(new Vector3(Random.Range(-1f, 1f) * width / 2, 0, Random.Range(-1f, 1f) * height / 2)));
            linerenderer.positionCount = 15;

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

        if (!usecoroutine)
        {
            Debug.Log("Start IronFiling");
            for (int i = 0; i < iterations * 2; i++)
            {
                Vector3 origin = new Vector3(Random.Range(-1f, 1f) * width / 2, 0, Random.Range(-1f, 1f) * height / 2);
                drawIron(origin, linerenderers[i]);
            }
            Debug.Log("End IronFiling");
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
            StopCoroutine(drawIronCoroutine());
            StartCoroutine(drawIronCoroutine());
        }


    }

    /// <summary>
    /// Draws a iron line starting at the given origin.
    /// </summary>
    /// <param name="origin">The origin to start</param>
    /// <param name="linerender">The line renderer to draw line</param>
    private void drawIron(Vector3 origin, LineRenderer linerender)
    {
        // line renderer points in world space!
        List<Vector3> linePoints = new List<Vector3> { transform.TransformPoint(origin) };

        Vector3 linePoint = origin;
        int numberOfPoints = 1;
        while (linePoint.x <= width / 2.0f && linePoint.x >= -width / 2.0f &&
               linePoint.z <= height / 2.0f && linePoint.z >= -height / 2.0f && numberOfPoints < maxvertexCount)
        {
            Vector3 globalLinePoint = transform.TransformPoint(linePoint);
            globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentLength;

            linePoints.Add(globalLinePoint);

            linePoint = transform.InverseTransformPoint(globalLinePoint);

            numberOfPoints++;
        }

        linerender.positionCount = numberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            linerender.SetPosition(i, linePoints[i]);
        }
    }

    private IEnumerator drawIronCoroutine()
    {
        Vector3 globalLeftUpBorderPoint = transform.TransformPoint(new Vector3(width / 2.0f, 0, height / 2.0f));
        Vector3 globalBottonDown = transform.TransformPoint(new Vector3(-width / 2.0f, 0, -height / 2.0f));

        //Caching values to maybe squeeze out a little bit more performance.
        float globalLeftUpBorderPointx = globalLeftUpBorderPoint.x;
        float globalBottonDownx = globalBottonDown.x;
        float globalLeftUpBorderPointz = globalLeftUpBorderPoint.z;
        float globalBottonDownz = globalBottonDown.z;
        Vector3 globalLinePoint;
        int numberOfPoints = 0;

        for (int i = 0; i < iterations * 2; i++)
        {
            // line renderer points in world space!
            globalLinePoint = linerenderers[i].GetPosition(0);


            linerenderers[i].positionCount = 15;
            for (numberOfPoints = 0; numberOfPoints < 15; numberOfPoints++)
            {
                globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentLength;
                linerenderers[i].SetPosition(numberOfPoints, globalLinePoint);
            }
        }
        yield return null;
        bool dirty = false;
        for (int n = 0; n < 25; n++)
        {
            for (int i = 0; i < iterations * 2; i++)
            {
                int positioncount = linerenderers[i].positionCount;
                if (positioncount > 3)
                {
                    globalLinePoint = linerenderers[i].GetPosition(positioncount - 1);
                }
                else
                {
                    globalLinePoint = transform.TransformPoint(new Vector3(Random.Range(-1f, 1f) * width / 2, 0, Random.Range(-1f, 1f) * height / 2));
                    positioncount = 0;
                }
                linerenderers[i].positionCount = maxvertexCount;
                for (numberOfPoints = positioncount; globalLinePoint.x <= globalLeftUpBorderPointx && globalLinePoint.x >= globalBottonDownx &&
                       globalLinePoint.z <= globalLeftUpBorderPointz && globalLinePoint.z >= globalBottonDownz && numberOfPoints < maxvertexCount; numberOfPoints++)
                {
                    globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentLength / 4.0f;
                    linerenderers[i].SetPosition(numberOfPoints, globalLinePoint);
                    dirty = true;
                }
                linerenderers[i].positionCount = numberOfPoints;
                linerenderers[i].Simplify(tolerance);
                if (dirty)
                {
                dirty = false;
                    yield return null;
                }
            }
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void resetObject()
    {
        StopCoroutine(drawIronCoroutine());
        foreach (LineRenderer linerenderer in linerenderers)
        {
            linerenderer.SetPosition(0, transform.TransformPoint(new Vector3(Random.Range(-1f, 1f) * width / 2, 0, Random.Range(-1f, 1f) * height / 2)));
            linerenderer.positionCount = 15;
        }
        gameObject.SetActive(false);
    }
}

//Archive:
/*
 *     private IEnumerator drawIronCoroutine()
    {
        Vector3 globalLeftUpBorderPoint = transform.TransformPoint(new Vector3(width / 2.0f, 0, height / 2.0f));
        Vector3 globalBottonDown = transform.TransformPoint(new Vector3(-width / 2.0f, 0, -height / 2.0f));

        //Caching values to maybe squeeze out a little bit more performance.
        float globalLeftUpBorderPointx = globalLeftUpBorderPoint.x;
        float globalBottonDownx = globalBottonDown.x;
        float globalLeftUpBorderPointz = globalLeftUpBorderPoint.z;
        float globalBottonDownz = globalBottonDown.z;
        Vector3 globalLinePoint;
        int numberOfPoints = 0;

        for (int i = 0; i < iterations * 2; i++)
        {
            // line renderer points in world space!
            globalLinePoint = transform.TransformPoint(new Vector3(Random.Range(-1f, 1f) * width / 2, 0, Random.Range(-1f, 1f) * height / 2));
            float lineSegmentFactor = lineSegmentLength;

            linerenderers[i].positionCount = maxvertexCount;
            float emergencystop = maxvertexCount * 2;
            for (numberOfPoints = 0; globalLinePoint.x <= globalLeftUpBorderPointx && globalLinePoint.x >= globalBottonDownx &&
                   globalLinePoint.z <= globalLeftUpBorderPointz && globalLinePoint.z >= globalBottonDownz && emergencystop > 0 && numberOfPoints < maxvertexCount; numberOfPoints++)
            {
                emergencystop--;
                linerenderers[i].SetPosition(numberOfPoints, globalLinePoint);
                globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentFactor;
                if (numberOfPoints >= 2)
                {
                    Vector3 A = linerenderers[i].GetPosition(numberOfPoints - 2);
                    Vector3 B = linerenderers[i].GetPosition(numberOfPoints - 1);
                    Vector3 C = linerenderers[i].GetPosition(numberOfPoints);
                    Vector2 A2 = new Vector2(A.x, A.y);
                    Vector2 B2 = new Vector2(B.x, B.y);
                    Vector2 C2 = new Vector2(C.x, C.y);
                    float Angle = Vector2.Angle(B2 - A2, C2 - B2) + 0.1f;
                    if ((C2 - B2).magnitude < (lineSegmentLength / 30.0f)) break;
                    Vector2 temp = C2 - B2;
                    if (Angle < maxAngle / 2)
                        lineSegmentFactor *= 2;
                    else if (Angle > maxAngle && lineSegmentFactor > lineSegmentLength / 10.0f)
                    {
                        lineSegmentFactor /= 2;
                        numberOfPoints -=2;
                        globalLinePoint = linerenderers[i].GetPosition(numberOfPoints);
                        globalLinePoint += Vector3.Normalize(field.get(globalLinePoint)) * lineSegmentFactor;
                    }
                    else
                        lineSegmentFactor *= 1.3f;
                }
            }
            linerenderers[i].SetPosition(numberOfPoints, globalLinePoint);
            linerenderers[i].positionCount = ++numberOfPoints;
            yield return null;
        }
        yield return null;
    }
    */