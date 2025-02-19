using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class CloseFieldLine : MonoBehaviour
{
    [Tooltip("Don't stop drawing until at least this many line segments have been drawn (i.e. the number of StopDrawing calls)")]
    [SerializeField] private int minLineSegmentCount = 3;
    [Tooltip("Stop drawing when this many FieldLine turning points have been reached")]
    [SerializeField] private int maxTurningPoints = 4;

    private FieldLine fieldLine;
    private int currentLineSegmentIndex;
    private float lastDistanceToStart;
    private float distanceToStartWhenDrawingShouldStop;

    /// <summary>
    /// Describes if the fieldline segment is approaching or receding from the FieldLine's Start Position, and how many times this has changed 
    /// (Not entierely accurate, but more or less the number of turning points the fieldLine had).
    /// An even value means the field Line segment is getting further away from the start pos, odd means coming closer to the start pos.
    /// Each time a fieldLine segment reaches a turning point (i.e. changes the approaching/receding status), the value will be increased by 1.
    /// E.g. At first the segments receded (0), then they got closer (1), then they receded again (2), then they got closer again (1)
    /// </summary>
    private int fieldLineTurningPoints;

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
        lastDistanceToStart = 0f;
        fieldLineTurningPoints = 0;
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

        // Check if distance to start pos got larger (receding from start pos)
        Vector3 fieldLineStartPosition = fieldLine.transform.TransformPoint(Vector3.zero - fieldLine.originOffset);
        float lineDistanceToStart = Vector3.Distance(lineSegmentPosition, fieldLineStartPosition);
        UpdateSegmentProperties(lineDistanceToStart);

        // Don't stop drawing when not at least minLineSegmentCount reached
        if (currentLineSegmentIndex < minLineSegmentCount)
            return false;

        if (PreviousSegmentRecededStartPos())
        {
            // Don't stop drawing when receding from start pos
            return false;
        }
        else
        {
            // Approaching start pos again, check if close enough
            if (lineDistanceToStart < distanceToStartWhenDrawingShouldStop)
            {
                return true;
            }
        }

        // If too many turning points reached, stop drawing
        if (fieldLineTurningPoints > maxTurningPoints)
            return true;

        // If very small field line loop, we also should stop when very close to Start even when we have not yet reached a smaller furthestDistanceToStart
        bool VeryCloseToStartPos = lineDistanceToStart <= (fieldLine.minLineSegmentLength * 2);
        return VeryCloseToStartPos;
    }

    private void UpdateSegmentProperties(float newLineDistanceToStart)
    {
        bool segmentIsNowReceding = newLineDistanceToStart > lastDistanceToStart;
        if (segmentIsNowReceding != PreviousSegmentRecededStartPos())
        {
            fieldLineTurningPoints++;
        }
        lastDistanceToStart = newLineDistanceToStart;
    }

    private bool PreviousSegmentRecededStartPos()
    {
        return fieldLineTurningPoints % 2 == 0;
    }
}
