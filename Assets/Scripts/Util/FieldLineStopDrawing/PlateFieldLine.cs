using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class PlateFieldLine : MonoBehaviour
{
    [SerializeField]
    private Collider targetPlateCollider;

    private void Start()
    {
        var fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 position)
    {
        return targetPlateCollider != null && targetPlateCollider.bounds.Contains(position);
    }
}
