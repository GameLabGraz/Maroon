using Maroon.Physics.Electromagnetism;
using UnityEngine;

[RequireComponent(typeof(FieldLine))]
public class CloseFieldLine : MonoBehaviour
{
    [SerializeField]
    private bool useCoilHack = false;

    private void Start()
    {
        var fieldLine = GetComponent<FieldLine>();
        if (fieldLine)
            fieldLine.stopDrawingCheck = StopDrawing;
    }

    private bool StopDrawing(Vector3 position)
    {
        var emObj = transform.parent.gameObject;

        var dist = transform.position;

        if (useCoilHack) // hack for coil
            dist -= 1.5f * new Vector3(Mathf.Abs(emObj.transform.up.x), Mathf.Abs(emObj.transform.up.y), Mathf.Abs(emObj.transform.up.z));

        return Vector3.Distance(position, dist) <= 0.053f ||
               Vector3.Distance(position, transform.position) <= 0.027f;
    }
}
