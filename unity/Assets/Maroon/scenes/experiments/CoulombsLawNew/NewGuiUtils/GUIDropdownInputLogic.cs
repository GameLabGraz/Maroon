using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GUILabelLogic))]
    public class GUIDropdownInputLogic : MonoBehaviour
    {
        [SerializeField] private int          initialSelectionIndex = 0;
        [SerializeField] private bool         isInteractable = true;
        [SerializeField] private string       nameLocalizationKey = "";
        [SerializeField] private List<string> dropdownOptions = new List<string>() { "Option A", "Option B" };

        // UI-Element references
        private TMPro.TMP_Dropdown dropdownField;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

        private void Start()
        {
            GetComponent<GUILabelLogic>().SetLabelData(nameLocalizationKey, "", SI_Prefix.NONE);

            // Initialize dropdown control
            dropdownField = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "DropdownControl").GetComponent<TMPro.TMP_Dropdown>();
            dropdownField.interactable = isInteractable;
            dropdownField.ClearOptions();
            dropdownField.AddOptions(dropdownOptions);
            dropdownField.value = initialSelectionIndex;
            dropdownField.onValueChanged.RemoveAllListeners();
            dropdownField.onValueChanged.AddListener((int newValue) => { OnValueChanged.Invoke(newValue); });
        }

        public int GetSelectedIndex() 
        {
            if (dropdownField == null)
            {
                return initialSelectionIndex;
            }
            return dropdownField.value; 
        }

        public void SetSelectedIndex(int index)
        {
            if (dropdownField != null)
            {
                dropdownField.value = index;
            }
            initialSelectionIndex = index;
        }
    }
}
