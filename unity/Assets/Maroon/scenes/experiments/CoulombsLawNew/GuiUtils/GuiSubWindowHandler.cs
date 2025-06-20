using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    // Note(MartinR): Currently doesn't do a lot, but I want to have a prefab, so that
    //      later on I can add e.g. an image or a button to collaps the subwindow
    public class GuiSubWindowHandler : MonoBehaviour
    {
        [SerializeField] private string subwindowTitle = "SubWindowTitle";

        // For aesthetic reasons similar UI elements (labels and input-fields) should have equal size in the subwindow.
        // See ComponentLabelHandler for how this value is used to achieve this.
        public float minimumLabelWidth      = 150;
        public float minimumInputFieldWidth = 60;
        public float minimumUnitLabelWidth  = 22;

        // Private references to UI-Elements
        private TMPro.TMP_Text titleLabel;
        private RectTransform contentPanel;

        private void Initialize()
        {
            titleLabel   = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "TitleText").GetComponent<TMPro.TMP_Text>();
            contentPanel = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "Content").GetComponent<RectTransform>();
            if (titleLabel == null || contentPanel == null)
            {
                Debug.LogWarning("GuiSubWindowHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }

            titleLabel.text = subwindowTitle;
        }

        private void Awake() { Initialize(); }
        private void OnValidate() { Initialize(); }
    }
}
