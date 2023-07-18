using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapController : MonoBehaviour
{
    public List<SnapPoint> snapPoints;
    public List<DragDrop> draggableObjects;
    public float snapRange = 0.5f;




    // Start is called before the first frame update
    void Start()
    {
        foreach(DragDrop draggable in draggableObjects)
        {
            draggable.dragEndedCallback = OnDragEnded;
        }
    }

    private void OnDragEnded(DragDrop draggable)
    {
        float closestDistance = -1;
        SnapPoint closestSnapPoint = null;

        foreach (SnapPoint snapPoint in snapPoints)
        {
            float currentDistance = Vector3.Distance(draggable.transform.position, snapPoint.transform.position);
            if(closestSnapPoint == null || currentDistance < closestDistance)
            {
                closestSnapPoint = snapPoint;
                closestDistance = currentDistance;
            }


        }
        if(closestSnapPoint != null && closestSnapPoint.currentObject == null && closestDistance <= snapRange)
        {
            draggable.transform.position = closestSnapPoint.transform.position;
            draggable.snapPoint = closestSnapPoint;
            closestSnapPoint.currentObject = draggable;
        }
    }

}
