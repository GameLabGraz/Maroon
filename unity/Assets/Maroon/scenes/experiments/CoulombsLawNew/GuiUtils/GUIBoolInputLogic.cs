using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GUILabelLogic))]
    public class GUIBoolInputLogic : MonoBehaviour
    {
        [SerializeField] private bool value = false;
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private string nameLocalizationKey = "";

        public UnityEngine.Events.UnityEvent<bool> OnValueChanged;

        private void Start()
        {
            GetComponent<GUILabelLogic>().SetLabelData(nameLocalizationKey, "", SI_Prefix.NONE);
            var toggle = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "Toggle").GetComponent<UnityEngine.UI.Toggle>();
            toggle.isOn = value;
            toggle.interactable = isInteractable;
            toggle.onValueChanged.AddListener((bool newValue) =>
            {
                value = newValue;
                OnValueChanged.Invoke(value);
            });
        }

        public bool GetValue() { return value; }

        public void SetValue(bool newValue) 
        {
            value = newValue;

            var toggle = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "Toggle").GetComponent<UnityEngine.UI.Toggle>();
            if (toggle != null)
            {
                toggle.isOn = newValue;
            }
        }
    }
}
