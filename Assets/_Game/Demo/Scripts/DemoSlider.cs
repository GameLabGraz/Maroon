using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Maroon {
  public class DemoSlider : Slider {
    [SerializeField]
    private TextMeshProUGUI text = null;

    protected override void Awake() {
      base.Awake();
      UpdateText();
    }

    protected override void UpdatePosition() {
      base.UpdatePosition();
      UpdateText();
    }

    private void UpdateText() {
      text.text = Value.ToString("#.00") + Environment.NewLine + "(" + (int)(Percent * 100) + "%)";
    }

  }
}
