using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class GuiBoolInputHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name:";
        [SerializeField] private bool _value = false;

        public UnityEngine.Events.UnityEvent<bool> OnValueChanged;

        // Private references to UI-Elements
        private UnityEngine.UI.Toggle toggle = null;

        private void Initialize()
        {
            // Find ui elements by name reference gameobjects by name
            var label  = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueName").GetComponent<TMPro.TMP_Text>();
            toggle = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "Toggle").GetComponent<UnityEngine.UI.Toggle>();
            if (label == null || toggle == null)
            {
                Debug.LogWarning("GuiBoolInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }

            // Initialize ui elements
            toggle.isOn = _value;
            label.text = valueName;

            // Register UI-Callbacks
            toggle.onValueChanged.RemoveAllListeners(); 
            toggle.onValueChanged.AddListener((bool newValue) =>
            {
                _value = newValue;
                OnValueChanged.Invoke(newValue);
            });
        }

        private void Awake() { Initialize(); }

        private void OnValidate() { Initialize(); }


        public bool GetValue() { return _value; }

        public void SetValue(bool newValue) {
            _value = newValue;
            // Note(MartinR): This check is required so SetValue can be called from other gameobjects in Awake
            if (toggle == null) return;
            toggle.isOn = newValue;
        }
    }
}
