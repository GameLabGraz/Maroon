using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragDrop : MonoBehaviour
{
  public delegate void DragEndedDelegate(DragDrop dragDrop);
  
  public DragEndedDelegate dragEndedCallback;
  public bool enabled = true;
  public bool snap = true;
  public Vector3 planePosition;
  private Vector3 worldPosition;
  Plane plane;
  private bool isDragged = false;
  public SnapPoint snapPoint = null;

  public bool axisLocked = false;
  public Axis axisLockedInto = Axis.X;

  private void Awake() {
    plane = new Plane(new Vector3(0,0,-1), planePosition);
  }
  private void OnMouseDown() 
  {
    if (!enabled)
    {
      return;
    }
    isDragged = true;
    if(snap && snapPoint)
    {
      snapPoint.currentObject = null;
      snapPoint = null;
    }
  }

  private void OnMouseDrag()
  {
    if (!enabled)
    {
      return;
    }
    float distance;
    if (isDragged)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (plane.Raycast(ray, out distance))
      {
        worldPosition = ray.GetPoint(distance);
      }
      
      if (axisLocked)
      {
        switch (axisLockedInto)
        {
          case Axis.X:
            transform.position = new Vector3(transform.position.x, worldPosition.y, worldPosition.z);
            break;
          case Axis.Y:
            transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
            break;
          case Axis.Z:
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            break;
        }
      }
      else
      {
        transform.position = worldPosition;
      }
    }
  }

  private void OnMouseUp()
  {
    if (!enabled)
    {
      return;
    }
    isDragged = false;
    if(snap)
    {
      dragEndedCallback(this);
    }
  }

}
