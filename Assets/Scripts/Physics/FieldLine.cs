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
	public IField field;

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
    /// The electro magnet object
    /// </summary>
    private GameObject emObj;

    /// <summary>
    /// Boolean value which indicates if the 
    /// field line is visible
    /// </summary>
    private bool visible = true;

    /// <summary>
    /// The line renderer which draws the line
    /// </summary>
    private AdvancedLineRenderer lineRenderer;

    public delegate bool StopDrawingCheck(Vector3 position);
    public StopDrawingCheck stopDrawingCheck;

    [SerializeField]
    private float lineWidth = 0.1f;

    /// <summary>
    /// Initializes the line renderer.
    /// </summary>
    public void Start()
    {
        lineRenderer = gameObject.GetComponent<AdvancedLineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<AdvancedLineRenderer>();

        lineRenderer.initLineRenderer();
        lineRenderer.SetWidth(lineWidth, lineWidth);

        emObj = transform.parent.gameObject;
    }

    /// <summary>
    /// Draws the field lines
    /// </summary>
    public void draw()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.Clear();

        if (!this.visible || Mathf.Abs(GetFieldStrengthFromEmObj()) < 0.05)
            return;

        float closingAngle = fixClosingAngle + (4 - GetFieldStrengthFromEmObj()) * 2;

        int positionIndex = 0; ;
        Vector3 position = transform.position - originOffset;
        lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
        positionIndex++;
        while (positionIndex < vertexCount)
        {
            Vector3 p = Vector3.Normalize(-field.get(position) * Teal.FieldStrengthFactor);

            Vector3 direction = new Vector3();
            direction.x = Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.x - Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.y;
            direction.y = Mathf.Sin(closingAngle * Mathf.Deg2Rad) * p.x + Mathf.Cos(closingAngle * Mathf.Deg2Rad) * p.y;
            direction.z = p.z;

            position += direction * lineSegmentLength;

            lineRenderer.SetPosition(positionIndex, transform.InverseTransformPoint(position));
            positionIndex++;

            if (stopDrawingCheck != null && stopDrawingCheck(position))
                break;
        }

        lineRenderer.WritePositionsToLineRenderer();

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
            clearLineRenderer();
    }

    /// <summary>
    /// Clears the clones and line renderer
    /// </summary>
    public void resetObject()
    {
        lineRenderer.Clear();
    }

    public List<KeyValuePair<int, Vector3>> GetLinePositions()
    {
        return lineRenderer.GetPositions();
    }

    private float GetFieldStrengthFromEmObj()
    {
        switch (field.getFieldType())
        {
            case FieldType.BField:
                return emObj.GetComponent<IGenerateB>().getFieldStrength();
            case FieldType.EField:
                return emObj.GetComponent<IGenerateE>().getFieldStrength();
            default:
                return 0;
        }
    }
}
