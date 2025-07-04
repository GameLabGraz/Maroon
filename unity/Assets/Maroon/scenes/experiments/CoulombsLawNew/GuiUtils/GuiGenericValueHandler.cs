using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [ExecuteAlways]
    public class GuiGenericValueHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name";
        [SerializeField] private string localizationKey = "";
        [SerializeField] private string unitName = "m";
        [SerializeField] private bool   useVerticalLayout = false;

        private GuiSubWindowHandler parentSubwindow;
        private UnityEngine.UI.VerticalLayoutGroup verticalLayoutPanel;
        private UnityEngine.UI.HorizontalLayoutGroup horizontalLayoutPanel;
        private GameObject[] valueNameLabels = new GameObject[2];
        private GameObject[] unitNameLabels = new GameObject[2];
        private GEAR.Localization.Text.LocalizedTMP[] nameLocalization = new GEAR.Localization.Text.LocalizedTMP[2];

        private void Awake()
        {
            parentSubwindow = GuiSubWindowHandler.FindParentSubWindow(gameObject);

            horizontalLayoutPanel = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "HorizontalLayout").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            verticalLayoutPanel   = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "VerticalLayout").GetComponent<UnityEngine.UI.VerticalLayoutGroup>();

            valueNameLabels[0]  = GuiSubWindowHandler.FindChildObjectByNameRecursive(horizontalLayoutPanel.gameObject, "ValueLabel");
            valueNameLabels[1]  = GuiSubWindowHandler.FindChildObjectByNameRecursive(verticalLayoutPanel.gameObject, "ValueLabel");
            nameLocalization[0] = GuiSubWindowHandler.FindChildObjectByNameRecursive(horizontalLayoutPanel.gameObject, "ValueLabel").GetComponent<GEAR.Localization.Text.LocalizedTMP>();
            nameLocalization[1] = GuiSubWindowHandler.FindChildObjectByNameRecursive(verticalLayoutPanel.gameObject, "ValueLabel").GetComponent<GEAR.Localization.Text.LocalizedTMP>();
            unitNameLabels[0]   = GuiSubWindowHandler.FindChildObjectByNameRecursive(horizontalLayoutPanel.gameObject, "UnitLabel");
            unitNameLabels[1]   = GuiSubWindowHandler.FindChildObjectByNameRecursive(verticalLayoutPanel.gameObject, "UnitLabel");

            horizontalLayoutPanel.gameObject.SetActive(!useVerticalLayout);
            verticalLayoutPanel.gameObject.SetActive(useVerticalLayout);
        }

        private void Start() { SetUIElementValues(); }

        private void SetUIElementValues()
        {
            if (horizontalLayoutPanel == null || verticalLayoutPanel == null || parentSubwindow == null) return;

            horizontalLayoutPanel.gameObject.SetActive(!useVerticalLayout);
            verticalLayoutPanel.gameObject.SetActive(useVerticalLayout);

            // Update labels
            for (int i = 0; i < 2; i++)
            {
                if (localizationKey.Length != 0)
                {
                    nameLocalization[i].Key = localizationKey;
                }
                else
                {
                    nameLocalization[i].enabled = false;
                    valueNameLabels[i].GetComponent<TMPro.TMP_Text>().text = valueName;
                }

                valueNameLabels[i].GetComponent<UnityEngine.UI.LayoutElement>().minWidth = parentSubwindow.minimumLabelWidth;
                unitNameLabels[i].GetComponent<TMPro.TMP_Text>().text = unitName;
                unitNameLabels[i].GetComponent<UnityEngine.UI.LayoutElement>().minWidth = parentSubwindow.minimumUnitLabelWidth;
                unitNameLabels[i].SetActive(unitName.Length != 0);
            }
        }

        private void OnValidate() { SetUIElementValues(); }

        public GameObject GetEmptyContentPanel() 
        { 
            var panel = 
                GuiSubWindowHandler.FindChildObjectByNameRecursive(
                    GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, useVerticalLayout ? "VerticalLayout" : "HorizontalLayout"), 
                    "Content");

            // Note(MartinR): I'm removing all children here so that the UI-code can use [ExecuteAlways],
            //      so it also get's updated correctly in the editor
            int childCount = panel.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (Application.isEditor)
                {
                    GameObject.DestroyImmediate(panel.transform.GetChild(0).gameObject);
                }
                else
                {
                    GameObject.Destroy(panel.transform.GetChild(0).gameObject);
                }
            }

            return panel;
        }
    }
}
