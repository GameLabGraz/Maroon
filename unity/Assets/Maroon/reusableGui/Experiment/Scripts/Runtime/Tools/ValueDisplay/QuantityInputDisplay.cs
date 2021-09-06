using UnityEngine;

namespace Maroon.UI.ValueDisplay
{
    [RequireComponent(typeof(InputField))]
    public class QuantityInputDisplay : QuantityValueDisplay
    {
        private InputField _valueDisplayField;

        protected new void Start()
        {
            base.Start();
            _valueDisplayField = GetComponent<InputField>();
            _valueDisplayField.interactable = false; // only for displaying the value.
        }

        protected override void UpdateDisplay(string value)
        {
            _valueDisplayField.text = value;
        }
    }
}