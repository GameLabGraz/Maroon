using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  
  public class MeasurableObject : MonoBehaviour
  {
    private float length_;
    public Axis measuredAxis;
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

    // Update is called once per frame
    void Update()
    {

    }
  }

  public enum Axis
  {
    X,
    Y,
    Z
  }
}
