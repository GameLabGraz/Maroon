using System.Globalization;
using UnityEngine;

namespace Maroon.Tools.Calculator.UI
{
    public abstract class CalculatorBaseDisplay : MonoBehaviour
    {
        [SerializeField] private Calculator calculator;

        public abstract string MainDisplayText { set; get; }
        public abstract string SideDisplayText { set; get; }

        private void Start()
        {
            Clear();

            calculator.OnResultChanged += OnResultChangedHandler;
            calculator.OnValueChanged += OnValueChangedHandler;
        }


        private void OnResultChangedHandler(Calculator.CalculatorValueChangedEvent ev)
        {
            if (ev.Op == "=")
            {
                MainDisplayText = ev.Value.ToString(CultureInfo.InvariantCulture);
                SideDisplayText = " ";
            }
            else
            {
                SideDisplayText = $"{ev.Value} {ev.Op}".ToString(CultureInfo.InvariantCulture);
                MainDisplayText = " ";
            }
        }

        private void OnValueChangedHandler(Calculator.CalculatorValueChangedEvent ev)
        {
            MainDisplayText = ev.Value.ToString(CultureInfo.InvariantCulture);
        }

        public void AppendNumber(string number)
        {
            MainDisplayText += number;
        }

        public void AddDecimalPoint()
        {
            if (!MainDisplayText.Contains("."))
                MainDisplayText += ".";
        }

        public void Clear()
        {
            SideDisplayText= " ";
            MainDisplayText = " ";
        }

        public void ClearEntry()
        {
            MainDisplayText = " ";
        }

        public void Erase()
        {
            MainDisplayText = MainDisplayText.Remove(MainDisplayText.Length - 1);
        }
    }
}
