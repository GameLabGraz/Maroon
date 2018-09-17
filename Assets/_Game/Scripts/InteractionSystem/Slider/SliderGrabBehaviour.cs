using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Maroon {
  public sealed class SliderGrabBehaviour : GrabBehaviour {

    private Slider Slider {
      get { return interactable as Slider; }
    }

    private void Awake() {
      Assert.IsNotNull(Slider, "This grab behaviour can only be applied to slider interactables.");
    }

    private void Update() {
      if (handle) {
        var totalSliderLen = Slider.TotalSliderLength;
        var sliderDir = Slider.SliderDirection;

        var handleVector = handle.Rigidbody.position - Slider.Min.point.position;
        var projectedHandlePos = Slider.Min.point.position + Vector3.Project(handleVector, sliderDir);

        var projectedHandleVector = projectedHandlePos - Slider.Min.point.position;
        var projectedHandleDir = projectedHandleVector.normalized;
        var positiveHandleDir = Vector3.Angle(projectedHandleDir, Slider.SliderDirection) < 90;

        if (positiveHandleDir) {
          var projectedHandleLen = (projectedHandlePos - Slider.Min.point.position).magnitude;
          if (projectedHandleLen > totalSliderLen) {
            Slider.Percent = 1;
          } else {
            Slider.Percent = projectedHandleLen / totalSliderLen;
          }
        } else {
          Slider.Percent = 0;
        }
      }
    }
  }
}
