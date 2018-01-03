//-----------------------------------------------------------------------------
// FieldLine.cs
//
// Script to draw the field lines
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Script to draw the field lines.
/// One Field Line is cloned around an axis.
/// </summary>
public class FieldLine : MonoBehaviour, IResetObject
{
    /// <summary>
    /// The magnetic field
    /// </summary>
    public BField field;

    /// <summary>
    /// The vertex count of a single field line.
    /// </summary>
    public int vertexCount = Teal.FieldLineVertexCount;

    /// <summary>
    /// The length of the line between two vertices
    /// </summary>
    public float lineSegmentLength = 0.3f;

    /// <summary>
    /// A certain offset from the origin where
    /// the first vertex is drawn.
    /// </summary>
    public Vector3 originOffset = Vector3.zero;

    /// <summary>
    /// Value to influence the curve of the field line
    /// </summary>
    public float fixClosingAngle = 14;

    /// <summary>
    /// Boolean value to indicate if the 
    /// field line belongs to a coil
    /// </summary>
    public bool coil = false;

    /// <summary>
    /// The electro magnet object
    /// </summary>
    private EMObject emObj;

    /// <summary>
    /// The symmetry axis around which the field lines
    /// are cloned
    /// </summary>
    private Vector3 symmetryAxis;

    /// <summary>
    /// Boolean value which indicates if the 
    /// field line is visible
    /// </summary>
    private bool visible = true;

    /// <summary>
    /// Number of clones to copy
    /// </summary>
    private int numClones;

    /// <summary>
    /// The line renderer which draws the line
    /// </summary>
    AdvancedLineRenderer lineRenderer;

    /// <summary>
    /// Set of existing field line clones
    /// </summary>
    private HashSet<GameObject> clones;

    /// <summary>
    /// Width of the drawn field lines
    /// </summary>
    public Vector2 lineWidth = new Vector2(0.1f, 0.1f);

    /// <summary>
    /// Initializes clones field
    /// </summary>
    public void Awake()
    {
        clones = new HashSet<GameObject>();
        numClones = Teal.DefaultNumFieldLines;
        symmetryAxis = new Vector3(0, 1, 0);

        lineRenderer = gameObject.GetComponent<AdvancedLineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<AdvancedLineRenderer>();
    }

    /// <summary>
    /// Initializes the line renderer.
    /// </summary>
    public void Start()
    {
        lineRenderer.initLineRenderer();
        lineRenderer.SetWidth(lineWidth.x, lineWidth.y);

        emObj = gameObject.GetComponentInParent<EMObject>();
    }

    /// <summary>
    /// Sets the symmetry axis and clones.
    /// </summary>
    /// <param name="count">number of clones</param>
    /// <param name="ax">symmetry axis</param>
    public void setSymmetry(int count, Vector3 ax)
    {
        numClones = count;
        symmetryAxis = ax;
    }

    /// <summary>
    /// Draws the field lines
    /// </summary>
    public void draw()
    {
        if (!visible)
            return;

        lineRenderer.Clear();
        clearClones();

        if (Mathf.Abs(emObj.getFieldStrength()) < 0.05)
            return;

        drawFieldLine();

        float rotationScale = 360f / numClones;

        float rotation = rotationScale;

        List<KeyValuePair<int, Vector3>> pointList = lineRenderer.GetPositions();
        for (int i = 1; i < numClones; ++i)
        {
            GameObject clone = Instantiate(gameObject, transform.position, Quaternion.identity);

            AdvancedLineRenderer lineRendererClone = clone.GetComponent<AdvancedLineRenderer>();
            lineRendererClone.SetVertexCount(pointList.Count);
            lineRendererClone.SetPositions(pointList);

            //workaround to keep the fieldline and its clones at the same scale
            Vector3 temp = clone.transform.localScale;
            clone.transform.SetParent(transform.parent);
            clone.transform.localScale = temp;
            //rotate clones to fill the whole 360°
            clone.transform.localEulerAngles = rotation * symmetryAxis;
            clones.Add(clone);
            rotation += rotationScale;
        }
    }

    /// <summary>
    /// Draws the first field line which
    /// is then copied
    /// </summary>
    private void drawFieldLine()
    {
        float closingAngle = fixClosingAngle + (4 - emObj.getFieldStrength()) * 2;

        int positionIndex = 0;

        Vector3 position = transform.position - originOffset;
        lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
        positionIndex++;
        while (positionIndex < vertexCount)
        {
            Vector3 p = Vector3.Normalize(-field.get(position) * Teal.FieldStrengthFactor);

            Vector3 direction = new Vector3
            (
                Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.x - Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.y,
                Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.x + Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.y,
                p.z
            );

            position += direction * lineSegmentLength;

            lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
            positionIndex++;

            Vector3 dist = transform.position;
            if (coil) // hack for coil
                dist -= 1.5f * new Vector3(Mathf.Abs(emObj.transform.up.x), Mathf.Abs(emObj.transform.up.y),
                            Mathf.Abs(emObj.transform.up.z));
            if (Vector3.Distance(position, dist) <= 0.8f || Vector3.Distance(position, transform.position) <= 0.4f)
                break;
        }

        lineRenderer.WritePositionsToLineRenderer();
    }

    /// <summary>
    /// Clears all the copies of the field line
    /// </summary>
    private void clearClones()
    {
        foreach (GameObject clone in clones)
            DestroyImmediate(clone);
        clones.Clear();
    }

    /// <summary>
    /// Clears the line renderer
    /// </summary>
    private void clearLineRenderer()
    {
        lineRenderer.Clear();
    }

    /// <summary>
    /// Sets the visibility of the field lines
    /// on or off
    /// </summary>
    /// <param name="visibility">visible if true else invisible</param>
    public void setVisibility(bool visibility)
    {
        this.visible = visibility;
        if (!visible)
        {
            clearLineRenderer();
            clearClones();
        }
    }

    /// <summary>
    /// Clears the clones and line renderer
    /// </summary>
    public void resetObject()
    {
        lineRenderer.Clear();
        clearClones();
    }
}