using UnityEngine;

public class CloseFieldLine : MonoBehaviour
{
    void Start()
    {
        FieldLine fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 position)
    {
        GameObject emObj = transform.parent.gameObject;

        Vector3 dist = transform.position;
        if (GetComponentInParent<Coil>())   // hack for coil
            dist -= 1.5f * new Vector3(Mathf.Abs(emObj.transform.up.x), Mathf.Abs(emObj.transform.up.y), Mathf.Abs(emObj.transform.up.z));
        if (Vector3.Distance(position, dist) <= 0.8f || Vector3.Distance(position, transform.position) <= 0.4f)
            return true;
        return false;
    }
}
