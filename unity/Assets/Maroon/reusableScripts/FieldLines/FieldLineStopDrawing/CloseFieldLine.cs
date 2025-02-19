using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class CloseFieldLine : MonoBehaviour
{
    private FieldLine fieldLine;

    [Tooltip("Don't stop drawing until at least this many line segments have been drawn (i.e. the number of StopDrawing calls)")]
    [SerializeField] private int minLineSegmentCount = 3;

    private int currentLineSegmentIndex;

    private void Awake()
    {
        fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
        {
            fieldLine.StartDrawing.AddListener(OnStartDrawing);
            fieldLine.stopDrawingCheck = StopDrawing;
        }
    }

    private void OnStartDrawing()
    {
        currentLineSegmentIndex = -1;
    }

    /// <summary>
    /// Checks if the FieldLine should stop drawing
    /// (e.g. because it is close to its start and would otherwise just loop unnecessarily)
    /// </summary>
    /// <param name="lineSegmentPosition">The position of the current line segment of the FieldLine</param>
    /// <returns>True if the FieldLine should stop drawing</returns>
    private bool StopDrawing(Vector3 lineSegmentPosition)
    {
        currentLineSegmentIndex++;

        if (currentLineSegmentIndex < minLineSegmentCount)
        {
            // Don't stop drawing until minLineSegmentCount has been reached
            return false;
        }

        Vector3 fieldLineStartPosition = fieldLine.transform.TransformPoint(Vector3.zero - fieldLine.originOffset);
        float lineDistanceToStart = Vector3.Distance(lineSegmentPosition, fieldLineStartPosition);
        bool closeToStart = lineDistanceToStart <= (fieldLine.minLineSegmentLength * 2);

        return closeToStart;
    }
}
