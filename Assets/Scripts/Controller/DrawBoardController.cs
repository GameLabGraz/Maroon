using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoardController : MonoBehaviour
{
    private AdvancedLineRenderer testLineRenderer;

    private Dictionary<PenController, LineRenderer> penLineRenderers;
    private Dictionary<PenController, Vector3> penPositionsOld;

    private void Start()
    {
        penLineRenderers = new Dictionary<PenController, LineRenderer>();
        penPositionsOld = new Dictionary<PenController, Vector3>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;

        PenController pen = other.GetComponent<PenController>();
        AddLineRendererForPen(pen);       
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;

        PenController pen = other.GetComponent<PenController>();

        LineRenderer lineRenderer = penLineRenderers[pen];
        
        Vector3 lineStartPosition = transform.InverseTransformPoint(penPositionsOld[pen]);
        lineRenderer.transform.localPosition = new Vector3(lineStartPosition.x, 0.004f, lineStartPosition.z);

        Vector3 lineEndPosition = lineRenderer.transform.InverseTransformPoint(other.transform.position);
        lineEndPosition.y = 0;


        Vector3 lineStartPositionWorld = lineRenderer.transform.position;
        Vector3 lineEndPositionWorld = lineRenderer.transform.TransformPoint(lineEndPosition);

        lineRenderer.SetPosition(0, lineStartPositionWorld);
        lineRenderer.SetPosition(1, lineEndPositionWorld);

        CapsuleCollider capsuleCollider = lineRenderer.GetComponent<CapsuleCollider>();
        capsuleCollider.transform.position = lineStartPositionWorld + (lineEndPositionWorld - lineStartPositionWorld) / 2;
        capsuleCollider.transform.LookAt(lineStartPositionWorld);
        capsuleCollider.height = (lineEndPositionWorld - lineStartPositionWorld).magnitude;

        AddLineRendererForPen(pen);
    }

    private void AddLineRendererForPen(PenController pen)
    {
        LineRenderer lineRenderer = new GameObject("DrawLine").AddComponent<LineRenderer>();
        lineRenderer.transform.parent = this.transform;
        lineRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        lineRenderer.material = pen.getPenMaterial();
        lineRenderer.startWidth = pen.getLineWidth();
        lineRenderer.endWidth = pen.getLineWidth();

        CapsuleCollider capsuleCollider = lineRenderer.gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
        capsuleCollider.radius = pen.getLineWidth() / 2;
        capsuleCollider.center = Vector3.zero;
        capsuleCollider.direction = 2;

        lineRenderer.gameObject.AddComponent<DrawLineController>();

        try
        {
            penLineRenderers.Add(pen, lineRenderer);
        }
        catch (ArgumentException)
        {
            penLineRenderers[pen] = lineRenderer;
        }

        try
        {
            penPositionsOld.Add(pen, pen.transform.position);
        }
        catch (ArgumentException)
        {
            penPositionsOld[pen] = pen.transform.position;
        }
    }
}
