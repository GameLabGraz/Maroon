using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointChargeExperiment
{
    [ExecuteAlways]
    public class UIBoolInput : MonoBehaviour
    {
        public bool value;
        public UnityEngine.Events.UnityEvent<bool> onBoolValueChanged;

        [SerializeField]
        private string _name;

        private UnityEngine.UI.Toggle _toggle;
        private TMPro.TMP_Text _labelText;

        private void InvokeValueChanged()
        {
            if (!Application.IsPlaying(gameObject)) return;
            onBoolValueChanged.Invoke(value);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Find child objects
            _toggle = transform.Find("Toggle").GetComponent<UnityEngine.UI.Toggle>();
            _labelText = transform.Find("Label").GetComponent<TMPro.TMP_Text>();

            // Invoke Value changed at start, so all listeners have the correct value
            InvokeValueChanged();

            // Set label text
            _labelText.text = _name + ":";

            // Set toggle value
            _toggle.isOn = value;
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool toggleValue)
        {
            value = toggleValue;
            InvokeValueChanged();
        }
    }
}
