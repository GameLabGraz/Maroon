using TMPro;
using UnityEngine;

namespace Maroon.UI.ValueDisplay
{
    [RequireComponent(typeof(TMP_Text))]
    public class QuantityTextDisplay : QuantityValueDisplay
    {
        private TMP_Text _valueDisplayField;
        protected new void Start()
        {
            base.Start();
            _valueDisplayField = GetComponent<TMP_Text>();
        }

        protected override void UpdateDisplay(string value)
        {
            _valueDisplayField.text = value;
        }
    }
}