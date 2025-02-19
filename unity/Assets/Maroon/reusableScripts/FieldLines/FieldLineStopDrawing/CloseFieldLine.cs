using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class CloseFieldLine : MonoBehaviour
{
    [Tooltip("Don't stop drawing until at least this many line segments have been drawn (i.e. the number of StopDrawing calls)")]
    [SerializeField] private int minLineSegmentCount = 3;

    private FieldLine fieldLine;
    private int currentLineSegmentIndex;
    private float furthestDistanceToStart;
    private float distanceToStartWhenDrawingShouldStop;

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
        // Reset field line's segment-related variables
        currentLineSegmentIndex = -1;
        furthestDistanceToStart = 0f;
        distanceToStartWhenDrawingShouldStop = ((fieldLine.minLineSegmentLength + fieldLine.maxLineSegmentLength) / 2f);
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

        // Check if distance to start pos got larger
        Vector3 fieldLineStartPosition = fieldLine.transform.TransformPoint(Vector3.zero - fieldLine.originOffset);
        float lineDistanceToStart = Vector3.Distance(lineSegmentPosition, fieldLineStartPosition);
        if (lineDistanceToStart > furthestDistanceToStart)
        {
            // distance to start position got larger
            furthestDistanceToStart = lineDistanceToStart;
        }

        if (currentLineSegmentIndex < minLineSegmentCount)
            // Don't stop drawing until at least minLineSegmentCount has been reached
            return false;

        if (lineDistanceToStart < furthestDistanceToStart)
        {
            // furthestDistanceToStart did not update
            // i.e. we might be already getting close to start pos
            if (lineDistanceToStart < distanceToStartWhenDrawingShouldStop)
            {
                return true;
            }
        }

        // If very small field line loop, we also should stop when very close to Start even when we have not yet reached a smaller furthestDistanceToStart
        bool VeryCloseToStartPos = lineDistanceToStart <= (fieldLine.minLineSegmentLength * 2);
        return VeryCloseToStartPos;
    }
}
