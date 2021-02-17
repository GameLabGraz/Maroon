using System.Collections.Generic;
using System.Linq;
using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class HitColliderFieldLine : MonoBehaviour
{
    [SerializeField] private List<Collider> ignoreCollider = new List<Collider>();
    private void Start()
    {
        var fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 position)
    {
        return Physics.OverlapSphere(position, 0.01f)
            .Where(hitCollider => !ignoreCollider.Contains(hitCollider))
            .Any(hitCollider => hitCollider.GetComponent<IGenerateB>() != null || 
                                hitCollider.GetComponent<IGenerateE>() != null);
    }
}
