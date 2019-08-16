using System;
using System.Linq;
using System.Globalization;
using UnityEngine;

namespace Maroon.Tools.Calculator
{
    public class Calculator : MonoBehaviour
    {
        public class CalculatorValueChangedEvent : EventArgs
        {
            public float Value { get; }

            public string Op { get; }

            public CalculatorValueChangedEvent(float value, string op)
            {
                Value = value;
                Op = op;
            }
        }

        private float _calculationValue;
        private float _result;

        private string _lastOperator;

        public delegate void CalculatorValueChanged(CalculatorValueChangedEvent ev);

        public static event CalculatorValueChanged OnResultChanged;
        public static event CalculatorValueChanged OnValueChanged;

        public void SetCalculationValue(float calculationValue)
        {
            _calculationValue = calculationValue;
        }

        public void SetCalculationValue(string calculationValue)
        {
            _calculationValue = ToFloat(calculationValue);
        }

        public void Clear()
        {
            _calculationValue = 0.0f;
            _result = 0.0f;
        }

        public void InvertSign()
        {
            _calculationValue *= -1;
            OnValueChanged?.Invoke(new CalculatorValueChangedEvent(_calculationValue, "±"));
        }

        public void Calculate(string op)
        {
            if (new[] { "sqrt", "sqr", "sin", "cos", "inv" }.Contains(op))
            {
                switch (op)
                {
                    case "sqrt":
                        _calculationValue = Mathf.Sqrt(_calculationValue);
                        break;

                    case "sqr":
                        _calculationValue *= _calculationValue;
                        break;

                    case "sin":
                        _calculationValue = Mathf.Sin(_calculationValue);
                        break;

                    case "cos":
                        _calculationValue = Mathf.Cos(_calculationValue);
                        break;

                    case "inv":
                        _calculationValue = 1.0f / _calculationValue;
                        break;
                }

                OnValueChanged?.Invoke(new CalculatorValueChangedEvent(_calculationValue, op));
            }

            else if (_lastOperator == null)
            {
                _lastOperator = op;
                _result = _calculationValue;

                OnResultChanged?.Invoke(new CalculatorValueChangedEvent(_result, op));
            }

            else if (new[] {"+", "-", "*", "/"}.Contains(op))
            {
                switch (_lastOperator)
                {
                    case "+":
                        _result += _calculationValue;
                        _calculationValue = 0;
                        break;

                    case "-":
                        _result -= _calculationValue;
                        _calculationValue = 0;
                        break;

                    case "*":
                        _result *= _calculationValue;
                        _calculationValue = 1;
                        break;

                    case "/":
                        _result /= _calculationValue;
                        _calculationValue = 1;
                        break;
                }

                _lastOperator = op;
                OnResultChanged?.Invoke(new CalculatorValueChangedEvent(_result, op));
            }
            else
            {
                Debug.LogError("Calculator: Invalid Operator");
            }
        }

        public void CalculateResult()
        {
            if (_lastOperator != null)
                Calculate(_lastOperator);
            else
                _result = _calculationValue;
                
            OnResultChanged?.Invoke(new CalculatorValueChangedEvent(_result, "="));

            _result = 0.0f;
            _lastOperator = null;
        }

        private static string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static float ToFloat(string value)
        {
            try
            {
                return float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch
            {
                return 0;
            }
        }
    }
}
