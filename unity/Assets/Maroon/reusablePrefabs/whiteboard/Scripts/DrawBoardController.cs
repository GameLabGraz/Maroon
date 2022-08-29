using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoardController : MonoBehaviour
{
    private Dictionary<PenController, Vector3> _penPositions = new Dictionary<PenController, Vector3>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;
 
        var pen = other.GetComponent<PenController>();
        AddPen(pen);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Pen"))
            return;

        var pen = other.GetComponent<PenController>();
        var drawLineController = AddDrawLineController(pen);

        var lineStartPosition = transform.InverseTransformPoint(_penPositions[pen]);
        drawLineController.transform.localPosition = new Vector3(lineStartPosition.x, 0.004f, lineStartPosition.z);

        var lineEndPosition = drawLineController.transform.InverseTransformPoint(other.transform.position);
        lineEndPosition.y = 0;

        var lineStartPositionWorld = drawLineController.transform.position;
        var lineEndPositionWorld = drawLineController.transform.TransformPoint(lineEndPosition);

        drawLineController.DrawLine(lineStartPositionWorld, lineEndPositionWorld);

        _penPositions[pen] = pen.transform.position;
    }

    private void AddPen(PenController pen)
    {
        try
        {
            _penPositions.Add(pen, pen.transform.position);
        }
        catch (ArgumentException)
        {
            _penPositions[pen] = pen.transform.position;
        }
    }
   
    private DrawLineController AddDrawLineController(PenController pen)
    {
        var drawLineConroller = new GameObject("DrawLine").AddComponent<DrawLineController>();
        drawLineConroller.transform.parent = this.transform;
        drawLineConroller.transform.localRotation = Quaternion.Euler(0, 0, 0);
        drawLineConroller.SetMaterial(pen.getPenMaterial());
        drawLineConroller.SetLineWidth(pen.getLineWidth());
        return drawLineConroller;
    }
}
