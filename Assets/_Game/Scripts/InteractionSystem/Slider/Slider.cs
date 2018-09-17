using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Maroon {
  public class Slider : Interactable {

    [Serializable]
    public class SliderPoint {
      public Transform point;
      public float value;
    }

    [SerializeField] protected SliderPoint min;
    [SerializeField] protected SliderPoint max;
    protected float percent = 0;


    public SliderPoint Min {
      get { return min; }
    }

    public SliderPoint Max {
      get { return max; }
    }

    public Vector3 SliderDirection {
      get { return (Max.point.position - Min.point.position).normalized; }
    }

    public float TotalSliderLength {
      get { return (Max.point.position - Min.point.position).magnitude; }
    }

    public float Percent {
      get { return percent; }
      set {
        percent = Mathf.Clamp01(value);
        UpdatePosition();
      }
    }

    public float Value {
      get { return Min.value + (Max.value - Min.value) * Percent; }
      set { Percent = (value - Min.value) / (Max.value - Min.value); }
    }

    protected override void Awake() {
      base.Awake();
      Assert.IsTrue(grabBehaviour is SliderGrabBehaviour,
        "Slider interactables must have a grab behaviour of type SliderGrabBehaviour!");
    }


    protected virtual void Start() {
      UpdatePosition();
    }

    protected virtual void UpdatePosition() {
      transform.position = Min.point.position + SliderDirection * TotalSliderLength * percent;
    }
  }
}
