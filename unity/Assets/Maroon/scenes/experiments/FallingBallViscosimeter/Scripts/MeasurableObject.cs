using System;
using System.Collections;
using System.Collections.Generic;
using Mirror.SimpleWeb;
using UnityEngine;

namespace Maroon.Physics
{
  
  public class MeasurableObject : MonoBehaviour
  {
    private MeasurementManager _measurementManager;
    private float length_;
    private bool clickable = false;
    
    
    public Axis measuredAxis;

    private void Awake()
    {
      _measurementManager = MeasurementManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
      if(measuredAxis == null)
      {
        Debug.LogWarning("No Axis Chosen!");
      }

      switch (measuredAxis){
        case Axis.X:
          length_ = gameObject.transform.localScale.x;
          break;
        case Axis.Y:
          length_ = gameObject.transform.localScale.y;
          break;
        case Axis.Z:
          length_ = gameObject.transform.localScale.z;
          break;
      }
    }

    public void makeChooseable()
    {
      clickable = true;
      Debug.Log("Hello");
      
      setDragDrop(false);
      //TODO: Enable Outline shader
    }

    public void resetChooseable()
    {
      clickable = false;

      setDragDrop(true);
    }

    private void setDragDrop(bool active)
    {
      DragDrop dragDrop = gameObject.GetComponent<DragDrop>();
      if (dragDrop)
      {
        dragDrop.enabled = active;
        Debug.Log(dragDrop.enabled);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
      Debug.Log("Clicked this Boy");
      if (!clickable)
      {
        return;
      }
      
      MeasurementManager.Instance.setChosenObject(this);
    }
  }

  public enum Axis
  {
    X,
    Y,
    Z
  }
}
