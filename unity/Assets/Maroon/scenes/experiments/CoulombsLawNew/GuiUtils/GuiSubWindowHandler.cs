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
        private UnityEngine.UI.Button headerButton;
        private UnityEngine.UI.Image collapsableImage;

        [SerializeField] private bool isCollapsed = false;
        [SerializeField] private bool collapseEnabled = true;

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
            headerButton = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "HeaderPanel").GetComponent<UnityEngine.UI.Button>();
            collapsableImage = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "CollapsableImage").GetComponent<UnityEngine.UI.Image>();

            if (titleLabel == null || contentPanel == null || headerButton == null || collapsableImage == null)
            {
                Debug.LogWarning("GuiSubWindowHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }

            collapsableImage.gameObject.SetActive(collapseEnabled);
            if (!collapseEnabled)
            {
                isCollapsed = false;
            }

            contentPanel.gameObject.SetActive(!isCollapsed);
            collapsableImage.rectTransform.rotation = Quaternion.Euler(0, 0, isCollapsed ? 180 : 0);

            headerButton.onClick.AddListener(() =>
            {
                if (!collapseEnabled) return;
                isCollapsed = !isCollapsed;
                contentPanel.gameObject.SetActive(!isCollapsed);
                collapsableImage.rectTransform.rotation = Quaternion.Euler(0, 0, isCollapsed ? 180 : 0);
            });
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
