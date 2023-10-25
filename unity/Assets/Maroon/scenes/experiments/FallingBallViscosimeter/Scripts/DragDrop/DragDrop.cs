using System;
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
  public bool offsetDragging = false;
  private Vector3 mouseOffset = Vector3.zero;
  public bool hoverSizeIncrease = true;
  public float hoverSizeFactor = 1.2f;
  public Vector3 planePosition;
  private Vector3 worldPosition;
  Plane plane;
  private bool isDragged = false;
  public SnapPoint snapPoint = null;

  public bool axisLocked = false;
  public Axis axisLockedInto = Axis.X;

  private PausableObject _pausableObject;
  private Vector3 oldScale;
  private void Awake() {
    plane = new Plane(new Vector3(0,0,-1), planePosition);
    _pausableObject = gameObject.GetComponent<PausableObject>();
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

    if (offsetDragging)
    {
      //get offset from center of object
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      float distance;
      Vector3 mousePosition;
      if (plane.Raycast(ray, out distance))
      {
        mousePosition = ray.GetPoint(distance);
        mouseOffset = transform.position - mousePosition;
      }
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
        worldPosition = ray.GetPoint(distance) + mouseOffset;
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
    if (_pausableObject)
    {
      _pausableObject.GetComponent<RigidBodyStateControl>().StoreRigidBodyState();
    }
    if(snap)
    {
      dragEndedCallback(this);
    }
  }

  private void OnMouseEnter()
  {
    if (hoverSizeIncrease)
    {
      oldScale = transform.localScale;
      transform.localScale = oldScale * hoverSizeFactor;
    }
  }

  private void OnMouseOver()
  {
    if (hoverSizeIncrease)
    {
      transform.localScale = oldScale * hoverSizeFactor;
    }
  }

  private void OnMouseExit()
  {
    if (hoverSizeIncrease)
    {
      transform.localScale = oldScale;
    }
  }
}
