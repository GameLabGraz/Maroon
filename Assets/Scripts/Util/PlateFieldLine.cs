using UnityEngine;

public class PlateFieldLine : MonoBehaviour
{
    [SerializeField]
    private Collider targetPlateCollider;

    void Start()
    {
        FieldLine fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 position)
    {
        if(targetPlateCollider != null)
            return targetPlateCollider.bounds.Contains(position);

        return false;
    }
}
