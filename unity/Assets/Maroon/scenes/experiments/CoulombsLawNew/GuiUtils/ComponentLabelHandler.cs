using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ComponentLabelHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name";
        [SerializeField] private string unitName = "m";

        private void Initialize()
        {
            GuiSubWindowHandler parentSubwindow = GuiFloatInputHandler.FindParentSubWindow(gameObject);

            var valueLabel = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueLabel");
            valueLabel.GetComponent<TMPro.TMP_Text>().text = valueName;
            valueLabel.GetComponent<UnityEngine.UI.LayoutElement>().minWidth = parentSubwindow.minimumLabelWidth;

            var unitLabel = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "UnitLabel");
            unitLabel.GetComponent<TMPro.TMP_Text>().text = unitName;
            unitLabel.GetComponent<UnityEngine.UI.LayoutElement>().minWidth = parentSubwindow.minimumUnitLabelWidth;
            unitLabel.SetActive(unitName.Length != 0);
        }

        private void Start() { Initialize(); }
        private void OnValidate() { Initialize(); }
    }
}
