using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class HitColliderFieldLine : MonoBehaviour
{
    private void Start()
    {
        var fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private static bool StopDrawing(Vector3 position)
    {
        return Physics.OverlapSphere(position, 0.01f).Any(hitCollider => hitCollider.GetComponent<EMObject>());
    }
}
