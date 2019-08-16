using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Tools.Calculator
{
    public class Display : MonoBehaviour
    {
        [SerializeField]
        private InputField _mainDisplay;

        [SerializeField]
        private Text _sideDisplay;


        private void Start()
        {
            Clear();

            Calculator.OnResultChanged += OnResultChangedHandler;
            Calculator.OnValueChanged += OnValueChangedHandler;
        }


        private void OnResultChangedHandler(Calculator.CalculatorValueChangedEvent ev)
        {
            if (ev.Op == "=")
            {
                _mainDisplay.text = ev.Value.ToString(CultureInfo.InvariantCulture);
                _sideDisplay.text = "";
            }               
            else
            {
                _sideDisplay.text = $"{ev.Value} {ev.Op}".ToString(CultureInfo.InvariantCulture);
                _mainDisplay.text = "";
            }
        }

        private void OnValueChangedHandler(Calculator.CalculatorValueChangedEvent ev)
        {
            _mainDisplay.text = ev.Value.ToString(CultureInfo.InvariantCulture);
        }

        public void SetMainDisplayText(string text)
        {
            _mainDisplay.text = text;
        }

        public void SetSideDisplayText(string text)
        {
            _sideDisplay.text = text;
        }

        public void AppendNumber(string number)
        {
            _mainDisplay.text += number;
        }

        public void AddDecimalPoint()
        {
            if (!_mainDisplay.text.Contains("."))
                _mainDisplay.text += ".";
        }

        public void Clear()
        {
            _sideDisplay.text = "";
            _mainDisplay.text = "";
        }

        public void ClearEntry()
        {
            _mainDisplay.text = "";
        }

        public void Erase()
        {
            _mainDisplay.text = _mainDisplay.text.Remove(_mainDisplay.text.Length - 1);
        }

    }
}
