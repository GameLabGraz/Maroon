using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    [ExecuteAlways]
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

        public static GameObject FindChildObjectByNameRecursive(GameObject gameObject, string name)
        {
            if (gameObject.name == name) return gameObject;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var foundObject = FindChildObjectByNameRecursive(gameObject.transform.GetChild(i).gameObject, name);
                if (foundObject != null) return foundObject;
            }
            return null;
        }

        public static GuiSubWindowHandler FindParentSubWindow(GameObject gameObject)
        {
            while (gameObject != null)
            {
                var subwindow = gameObject.GetComponent<GuiSubWindowHandler>();
                if (subwindow != null) return subwindow;
                if (gameObject.transform.parent == null) return null;
                gameObject = gameObject.transform.parent.gameObject;
            }
            return null;
        }

        private void Awake()
        {
            titleLabel   = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "TitleText").GetComponent<TMPro.TMP_Text>();
            contentPanel = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "Content").GetComponent<RectTransform>();
            if (titleLabel == null || contentPanel == null)
            {
                Debug.LogWarning("GuiSubWindowHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }
        }
        private void Start() { SetUIElementValues(); }

        private void SetUIElementValues()
        {
            if (titleLabel == null) return;
            titleLabel.text = subwindowTitle;
        }

        private void OnValidate() { SetUIElementValues(); }
    }
}
