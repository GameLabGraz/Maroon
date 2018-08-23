using System.Collections;
using System.Collections.Generic;
using Maroon;
using TMPro;
using UnityEngine;
using VRTK.Highlighters;

namespace Maroon {
    public class ContinuousUseDemoButton : Interactable {

        [SerializeField]
        private TextMeshProUGUI text = null;

        private float timeAtStartUse;

        protected override void Awake() {
            base.Awake();
            UpdateText();
        }

        public override void StartUse() {
            base.StartUse();
            timeAtStartUse = Time.time;
            UpdateText();
        }

        protected override void Use() {
            base.Use();
            UpdateText();
        }

        public override void StopUse() {
            base.StopUse();
            UpdateText();
        }

        private void UpdateText() {
            if (inUse) {
                text.text = "Use Duration: " + (Time.time - timeAtStartUse) + "s";
            }
            else {
                text.text = "Not in Use";
            }
        }
    }
}