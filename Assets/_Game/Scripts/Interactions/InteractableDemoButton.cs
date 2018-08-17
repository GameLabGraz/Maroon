using System.Collections;
using System.Collections.Generic;
using Maroon;
using TMPro;
using UnityEngine;
using VRTK.Highlighters;

namespace Maroon {
    public class InteractableDemoButton : Interactable {

        [SerializeField]
        private TextMeshProUGUI text = null;

        [SerializeField]
        private Animator animator = null;

        private string textPrefix;
        private int interactionCounter = 0;

        private void Awake() {
            textPrefix = text.text.TrimEnd(' ');
            UpdateText();
        }

        public override void Interact() {
            interactionCounter++;
            UpdateText();
            animator.SetTrigger("press");
        }

        private void UpdateText() {
            text.text = textPrefix + " " + interactionCounter;
        }
    }
}