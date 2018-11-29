using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoardController : MonoBehaviour
{
    private Dictionary<PenController, Vector3> penPositions;

    private void Start()
    {
        penPositions = new Dictionary<PenController, Vector3>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;
 
        PenController pen = other.GetComponent<PenController>();
        AddPen(pen);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;

        PenController pen = other.GetComponent<PenController>();
        DrawLineController drawLineController = AddDrawLineController(pen);

        Vector3 lineStartPosition = transform.InverseTransformPoint(penPositions[pen]);
        drawLineController.transform.localPosition = new Vector3(lineStartPosition.x, 0.004f, lineStartPosition.z);

        Vector3 lineEndPosition = drawLineController.transform.InverseTransformPoint(other.transform.position);
        lineEndPosition.y = 0;

        Vector3 lineStartPositionWorld = drawLineController.transform.position;
        Vector3 lineEndPositionWorld = drawLineController.transform.TransformPoint(lineEndPosition);

        drawLineController.DrawLine(lineStartPositionWorld, lineEndPositionWorld);

        penPositions[pen] = pen.transform.position;
    }

    private void AddPen(PenController pen)
    {
        try
        {
            penPositions.Add(pen, pen.transform.position);
        }
        catch (ArgumentException)
        {
            penPositions[pen] = pen.transform.position;
        }
    }
   
    private DrawLineController AddDrawLineController(PenController pen)
    {
        DrawLineController drawLineConroller = new GameObject("DrawLine").AddComponent<DrawLineController>();
        drawLineConroller.transform.parent = this.transform;
        drawLineConroller.transform.localRotation = Quaternion.Euler(0, 0, 0);
        drawLineConroller.SetMaterial(pen.getPenMaterial());
        drawLineConroller.SetLineWidth(pen.getLineWidth());
        return drawLineConroller;
    }
}
