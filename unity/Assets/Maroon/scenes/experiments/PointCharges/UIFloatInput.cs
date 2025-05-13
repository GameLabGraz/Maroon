using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointChargeExperiment
{
    [ExecuteAlways]
    public class UIFloatInput : MonoBehaviour
    {
        public float value;
        public UnityEngine.Events.UnityEvent<float> onFloatValueChanged;
        public UnityEngine.Events.UnityEvent<int> onIntValueChanged;

        [SerializeField]
        private string _name;
        [SerializeField]
        private float _minValue;
        [SerializeField]
        private float _maxValue;
        [SerializeField]
        private bool _integerMode;

        private TMPro.TMP_InputField _inputField;
        private UnityEngine.UI.Slider _slider;
        private TMPro.TMP_Text _labelText;

        private void InvokeValueChanged()
        {
            if (!Application.IsPlaying(gameObject)) return;

            if (_integerMode)
            {
                onIntValueChanged.Invoke((int)value);
            }
            else
            {
                onFloatValueChanged.Invoke(value);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Find child objects
            _inputField = transform.Find("InputField").GetComponent<TMPro.TMP_InputField>();
            _slider = transform.Find("SliderPanel/Slider").GetComponent<UnityEngine.UI.Slider>();
            _labelText = transform.Find("VertCenterPanel/LabelText").GetComponent<TMPro.TMP_Text>();

            // Invoke Value changed at start, so all listeners have the correct value
            value = Mathf.Clamp(value, _minValue, _maxValue);
            InvokeValueChanged();

            // Set label infos
            _labelText.text = _name + ":";

            // Set slider values
            _slider.minValue = _minValue;
            _slider.maxValue = _maxValue;
            _slider.value = value;
            _slider.wholeNumbers = _integerMode;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);

            // Set input field values
            _inputField.contentType = _integerMode ? TMPro.TMP_InputField.ContentType.IntegerNumber : TMPro.TMP_InputField.ContentType.DecimalNumber;
            _inputField.text = _integerMode ? ((int)(value)).ToString("D") : value.ToString("F");
            _inputField.onEndEdit.AddListener(OnTextInputValueChanged);
        }

        private void OnSliderValueChanged(float sliderValue)
        {
            if (!Application.IsPlaying(gameObject)) return;

            if (sliderValue == value) return;
            value = Mathf.Clamp(sliderValue, _minValue, _maxValue);
            _inputField.text = _integerMode ? ((int)(value)).ToString("D") : value.ToString("F");
            InvokeValueChanged();
        }

        private void OnTextInputValueChanged(string text)
        {
            if (!Application.IsPlaying(gameObject)) return;

            if (_integerMode)
            {
                int parseValue = 0;
                if (int.TryParse(text, out parseValue))
                {
                    value = (float)parseValue;
                }
            }
            else
            {
                float parseValue = 0.0f;
                if (float.TryParse(text, out parseValue))
                {
                    value = parseValue;
                }
            }

            value = Mathf.Clamp(value, _minValue, _maxValue);
            _slider.value = value; // Does this trigger OnSliderValueChanged?

            _inputField.text = _integerMode ? ((int)(value)).ToString("D") : value.ToString("F");
            InvokeValueChanged();
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
