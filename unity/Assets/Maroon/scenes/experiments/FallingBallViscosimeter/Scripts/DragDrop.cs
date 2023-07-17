using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragDrop : MonoBehaviour
{
  private Vector3 starting_z_value;

  public Vector3 worldPosition;
  Plane plane = new Plane(new Vector3(0,0,-1), new Vector3(0,0,2));


  private void Awake()
  {
    starting_z_value = new Vector3(0, 0, transform.position.z);
  }

  private void OnMouseDown() 
  {
    Debug.Log("OnMouseDown");
  }

  private void OnMouseDrag() {
    float distance;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (plane.Raycast(ray, out distance))
    {
      worldPosition = ray.GetPoint(distance);
      Debug.Log(worldPosition);
    }
    transform.position = worldPosition;
  }

}
