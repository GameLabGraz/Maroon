using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [ExecuteAlways]
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiBoolInputHandler : MonoBehaviour
    {
        [SerializeField] private bool initialValue = false;
        [SerializeField] private bool isInteractable = true;

        private bool _value = false;
        public UnityEngine.Events.UnityEvent<bool> OnValueChanged;

        // Private references to UI-Elements
        private UnityEngine.UI.Toggle toggle = null;

        private void Awake()
        {
            // Create UI elements by instanciating prefab
            var contentPanel = GetComponent<GuiGenericValueHandler>().GetEmptyContentPanel();
            var boolInputPrefab = Resources.Load("GuiBoolInputPrefab");
            var boolInputObject = (GameObject) GameObject.Instantiate(boolInputPrefab, contentPanel.transform);

            // Find child-ui-elements by name
            toggle = GuiSubWindowHandler.FindChildObjectByNameRecursive(boolInputObject, "Toggle").GetComponent<UnityEngine.UI.Toggle>();
            if (toggle == null)
            {
                Debug.LogWarning("GuiBoolInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }
        }

        private void Start()
        {
            // Register UI-Callbacks
            toggle.onValueChanged.RemoveAllListeners(); 
            toggle.onValueChanged.AddListener((bool newValue) =>
            {
                _value = newValue;
                OnValueChanged.Invoke(newValue);
            });

            SetUIElementValues();
            SetValue(initialValue);
        }

        void SetUIElementValues()
        {
            if (toggle == null) return;
            toggle.isOn = _value;
            toggle.interactable = isInteractable;
        }

        private void OnValidate() { SetUIElementValues(); SetValue(initialValue); }



        public bool GetValue() { return _value; }

        public void SetValue(bool newValue) 
        {
            _value = newValue;
            if (toggle == null) return;
            toggle.isOn = newValue;
        }

        // Note: See notes in GuiFloatInputHandler.SetInitialValue
        public void SetInitialValue(bool initialValue)
        {
            if (Application.isEditor && !Application.isPlaying) return;
            this.initialValue = initialValue;
            SetValue(initialValue);
        }
    }
}
