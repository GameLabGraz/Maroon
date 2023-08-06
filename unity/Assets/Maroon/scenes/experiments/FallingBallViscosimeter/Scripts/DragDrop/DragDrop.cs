using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragDrop : MonoBehaviour
{
  public delegate void DragEndedDelegate(DragDrop dragDrop);

  public DragEndedDelegate dragEndedCallback;


  public Vector3 worldPosition;
  Plane plane = new Plane(new Vector3(0,0,-1), new Vector3(0,0,2));

  private bool isDragged = false;
  public SnapPoint snapPoint = null;

  private void OnMouseDown() 
  {
    isDragged = true;
    snapPoint.currentObject = null;
    snapPoint = null;
  }

  private void OnMouseDrag()
  {
    float distance;
    if (isDragged)
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (plane.Raycast(ray, out distance))
      {
        worldPosition = ray.GetPoint(distance);
      }
      transform.position = worldPosition;
    }
  }

  private void OnMouseUp()
  {
    isDragged = false;
    dragEndedCallback(this);
  }

}
