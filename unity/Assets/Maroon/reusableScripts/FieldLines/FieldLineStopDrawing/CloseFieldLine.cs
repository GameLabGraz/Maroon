using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class CloseFieldLine : MonoBehaviour
{
    private FieldLine fieldLine;

    private void Awake()
    {
        fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 lineSegmentPosition)
    {
        var emObj = transform.parent.gameObject;

        var dist = transform.position;

        Vector3 fieldLineOffsetStart = fieldLine.transform.TransformPoint(Vector3.zero - fieldLine.originOffset);
        float lineDistanceToStart = Vector3.Distance(lineSegmentPosition, fieldLineOffsetStart);
        bool closeToStart = lineDistanceToStart <= (fieldLine.minLineSegmentLength * 2);
        return closeToStart;
    }
}
