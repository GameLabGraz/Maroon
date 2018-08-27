using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Maroon
{
  public sealed class SliderGrabAttach : GrabAttach
  {

    private Slider Slider {
      get { return interactable as Slider; }
    }

    private void Awake()
    {
      Assert.IsNotNull(Slider, "This grab behaviour can only be applied to slider interactables.");
    }

    private void Update()
    {
      if (handle)
      {
        var sliderVector = Slider.Max.point.position - Slider.Min.point.position;
        var totalSliderLen = Slider.TotalSliderLength;
        var sliderDir = Slider.SliderDirection;

        var handleVector = handle.Rigidbody.position - Slider.Min.point.position;
        var sliderPos = Slider.Min.point.position + Vector3.Project(handleVector, sliderDir);
        var handleDir = (sliderPos - Slider.Min.point.position).normalized;
        var positiveHandleDir = Vector3.Angle(handleDir, Slider.SliderDirection) < 90;
        var sliderLenFromMin = (sliderPos - Slider.Min.point.position).magnitude;
        var sliderLen = !positiveHandleDir
          ? 0
          : (sliderLenFromMin > totalSliderLen ? totalSliderLen : sliderLenFromMin);
        Slider.Percent = sliderLen / totalSliderLen;
      }
    }
  }
}
