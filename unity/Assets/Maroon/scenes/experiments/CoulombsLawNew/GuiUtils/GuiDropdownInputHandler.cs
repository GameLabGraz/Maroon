using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiDropdownInputHandler : MonoBehaviour
    {
        [SerializeField] private int initialSelectionIndex = 0;
        [SerializeField] private List<string> dropdownOptions = new List<string>() { "Option A", "Option B" };

        // UI-Element references
        private GuiSubWindowHandler parentSubwindow;
        private TMPro.TMP_Dropdown dropdownField;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

        private void Awake()
        {
            // Create UI elements by instanciating prefab
            var contentPanel = GetComponent<GuiGenericValueHandler>().GetEmptyContentPanel();
            var inputPrefab = Resources.Load("GuiDropDownInputPrefab");
            var inputObject = (GameObject)GameObject.Instantiate(inputPrefab, contentPanel.transform);

            // Find ui elements by name
            parentSubwindow = GuiSubWindowHandler.FindParentSubWindow(gameObject);
            dropdownField = inputObject.GetComponent<TMPro.TMP_Dropdown>();

            if (dropdownField == null)
            {
                Debug.LogWarning("GuiDropdownInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }
        }

        private void Start()
        {
            dropdownField.ClearOptions();
            dropdownField.AddOptions(dropdownOptions);
            dropdownField.value = initialSelectionIndex;
            dropdownField.onValueChanged.AddListener((int newValue) => { OnValueChanged.Invoke(newValue); });
        }

        public int GetSelectedIndex() { return dropdownField.value; }
    }
}
