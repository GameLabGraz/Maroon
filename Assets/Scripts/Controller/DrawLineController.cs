using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Eraser"))
            return;

        Destroy(this.gameObject);
    }
}
