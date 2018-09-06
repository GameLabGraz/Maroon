using System.Collections;
using System.Collections.Generic;
using Maroon;
using TMPro;
using UnityEngine;
using VRTK.Highlighters;

namespace Maroon {
  public class SingleUseDemoButton : Interactable {

    [SerializeField]
    private TextMeshProUGUI text = null;

    private int useCounter = 0;

    protected override void Awake() {
      base.Awake();
      UpdateText();
    }

    public override void StartUse() {
      base.StartUse();
      useCounter++;
      UpdateText();
    }

    private void UpdateText() {
      text.text = "Nr. of Uses: " + useCounter;
    }
  }
}