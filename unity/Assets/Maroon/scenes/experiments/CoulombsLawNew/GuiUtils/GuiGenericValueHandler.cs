using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [ExecuteAlways]
    public class GuiGenericValueHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name";
        [SerializeField] private string unitName = "m";

        private GuiSubWindowHandler parentSubwindow;
        TMPro.TMP_Text valueNameLabel;
        UnityEngine.UI.LayoutElement valueNameLayoutElement;
        TMPro.TMP_Text unitNameLabel;
        UnityEngine.UI.LayoutElement unitNameLayoutElement;

        private void Awake()
        {
            parentSubwindow = GuiSubWindowHandler.FindParentSubWindow(gameObject);

            var valueLabel = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "ValueLabel");
            valueNameLabel = valueLabel.GetComponent<TMPro.TMP_Text>();
            valueNameLayoutElement = valueLabel.GetComponent<UnityEngine.UI.LayoutElement>();

            var unitLabel = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "UnitLabel");
            unitNameLabel = unitLabel.GetComponent<TMPro.TMP_Text>();
            unitNameLayoutElement = unitLabel.GetComponent<UnityEngine.UI.LayoutElement>();
        }

        private void Start() { SetUIElementValues(); }

        private void SetUIElementValues()
        {
            if (valueNameLabel == null || unitNameLabel == null || parentSubwindow == null) return;

            valueNameLabel.text = valueName;
            valueNameLayoutElement.minWidth = parentSubwindow.minimumLabelWidth;

            unitNameLabel.text = unitName;
            unitNameLayoutElement.minWidth = parentSubwindow.minimumUnitLabelWidth;
            unitNameLayoutElement.gameObject.SetActive(unitName.Length != 0);
        }

        private void OnValidate() { SetUIElementValues(); }

        public GameObject GetEmptyContentPanel() 
        { 
            var panel = GuiSubWindowHandler.FindChildObjectByNameRecursive(gameObject, "Content");

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
