using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
        capsuleCollider.center = Vector3.zero;
        capsuleCollider.direction = 2; // z-axis
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Eraser"))
            return;

        Destroy(this.gameObject);
    }

    public void DrawLine(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        capsuleCollider.transform.position = startPosition + (endPosition - startPosition) / 2;
        capsuleCollider.transform.LookAt(startPosition);
        capsuleCollider.height = (endPosition - startPosition).magnitude;
    }

    public void SetMaterial(Material lineMaterial)
    {
        lineRenderer.material = lineMaterial;
    }

    public void SetLineWidth(float lineWidth)
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        capsuleCollider.radius = lineWidth / 2;
    }

    public LineRenderer GetLineRenderer()
    {
        return this.lineRenderer;
    }
}
