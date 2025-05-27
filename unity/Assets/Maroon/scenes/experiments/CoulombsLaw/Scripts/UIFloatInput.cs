using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLaw
{
    public class UIFloatInput : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string valueName = "Name:";
        [SerializeField] private string unitName = "m";
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 1;
        [SerializeField] private float initialValue = 0.5f;
        [SerializeField] private int postCommaDigits = 2;

        [Header("References to UI-Objects")]
        [SerializeField] private TMPro.TMP_Text nameLabel;
        [SerializeField] private TMPro.TMP_Text unitLabel;
        [SerializeField] private TMPro.TMP_InputField inputField;
        [SerializeField] private UnityEngine.UI.Slider slider;
        [SerializeField] private UnityEngine.UI.LayoutElement inputFieldLayoutElement;

        private float value = 0.5f;
        public UnityEngine.Events.UnityEvent<float> OnValueChanged;

        public float GetValue() { return value; }

        public void SetValue(float newValue)
        {
            newValue = Mathf.Clamp(newValue, minValue, maxValue);
            value = newValue;

            string formatString = "0";
            if (postCommaDigits > 0)
            {
                formatString += ".";
                for (int i = 0; i < postCommaDigits; i++)
                {
                    formatString += "0";
                }
            }

            inputField.text = value.ToString(formatString);
            if (slider.value != newValue)
            {
                slider.value = newValue;
            }
        }
            
        private void Awake()
        {
            value = initialValue;
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            SetValue(initialValue);

            const float APPROXIMATE_CHAR_WIDTH = 24; // Note(MartinR): Just an approximation with font size 36
            int expectedPreCommaDigits = (int) Mathf.Max(1.0f, Mathf.Log10(Mathf.Max(Mathf.Abs(minValue), Mathf.Abs(maxValue))));
            // Text-Field minWidth depends on all characters, including the dot "." and a possible "-" at the start
            inputFieldLayoutElement.minWidth = 
                APPROXIMATE_CHAR_WIDTH * (expectedPreCommaDigits + postCommaDigits + (postCommaDigits > 0 ? 1 : 0) + (minValue < 0 ? 1 : 0));

            nameLabel.text = valueName;
            unitLabel.text = unitName;
            if (unitName.Length == 0)
            {
                unitLabel.gameObject.SetActive(false);
            }

            slider.onValueChanged.AddListener((float newValue) =>
            {
                SetValue(newValue);
                OnValueChanged.Invoke(newValue);
            });

            inputField.onEndEdit.AddListener((string text) =>
            {
                float newValue = value;
                bool parseSuccess = float.TryParse(text, out newValue);
                if (!parseSuccess)
                {
                    newValue = value;
                }
                SetValue(newValue);
                OnValueChanged.Invoke(newValue);
            });
        }
    }
}
