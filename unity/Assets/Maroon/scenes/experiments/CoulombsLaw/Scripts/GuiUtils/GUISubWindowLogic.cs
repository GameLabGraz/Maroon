using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEAR.Localization.Text;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GUISubwindowLogic : MonoBehaviour
    {
        [SerializeField] private string titleLocalizationKey = "";
        [SerializeField] private bool isCollapsed = false;
        [SerializeField] private bool collapseEnabled = true;

        // For aesthetic reasons similar UI elements (labels and input-fields) should have equal size in the subwindow.
        // See ComponentLabelHandler for how this value is used to achieve this.
        public float minimumLabelWidth      = 150;
        public float minimumInputFieldWidth = 60;
        public float minimumUnitLabelWidth  = 22;

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

        public static GUISubwindowLogic FindParentSubWindow(GameObject gameObject)
        {
            while (gameObject != null)
            {
                var subwindow = gameObject.GetComponent<GUISubwindowLogic>();
                if (subwindow != null) return subwindow;
                if (gameObject.transform.parent == null) return null;
                gameObject = gameObject.transform.parent.gameObject;
            }
            return null;
        }

        private void Start() 
        {
            var titleLabel       = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "TitleText").GetComponent<TMPro.TMP_Text>();
            var localizedTitle   = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "TitleText").GetComponent<LocalizedTMP>();
            var contentPanel     = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "Content").GetComponent<RectTransform>();
            var headerButton     = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "HeaderPanel").GetComponent<UnityEngine.UI.Button>();
            var collapsableImage = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "CollapsableImage").GetComponent<UnityEngine.UI.Image>();
            if (titleLabel == null || contentPanel == null || headerButton == null || collapsableImage == null)
            {
                Debug.LogWarning("Could not find all gui references, is prefab used properly?");
                return;
            }

            localizedTitle.Key = titleLocalizationKey;

            // Enable/Disable collapsed image
            collapsableImage.gameObject.SetActive(collapseEnabled);
            if (!collapseEnabled)
            {
                isCollapsed = false;
            }

            // Collapse logic
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
    }
}
