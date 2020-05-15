using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class TextDisplay : ValueDisplay
    {
        [SerializeField]
        private string _unit;

        [SerializeField]
        private int _decimalPlaces;

        [SerializeField]
        private bool _useDecimalPlaceLimit = false;

        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void FixedUpdate()
        {
            var value = _useDecimalPlaceLimit
                ? (float) Math.Round(GetValue<float>(), _decimalPlaces)
                : GetValue<float>();

            _text.text = value + " " + _unit;
        }
    }
}
