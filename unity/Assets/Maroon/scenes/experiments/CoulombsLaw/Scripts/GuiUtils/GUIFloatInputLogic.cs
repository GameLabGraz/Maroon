using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    [RequireComponent(typeof(GUILabelLogic))]
    public class GUIFloatInputLogic : MonoBehaviour
    {
        [SerializeField] private float     value = 1;
        [SerializeField] private bool      isInteractable = true;
        [SerializeField] private string    nameLocalizationKey = "";
        [SerializeField] private string    unitName = "";
        [SerializeField] private SI_Prefix prefix = SI_Prefix.NONE;
        [SerializeField] private int       postCommaDigits = 2;
        [SerializeField] private bool      clampToMinMax = true;
        [SerializeField] private float     minValue = 1;
        [SerializeField] private float     maxValue = 10;
        [SerializeField] private bool      sliderEnabled = true;
        [SerializeField] private bool      textInputEnabled = true;

        public UnityEngine.Events.UnityEvent<float> OnValueChanged;

        // UI-Element references
        private GUISubwindowLogic parentSubwindow;
        private TMPro.TMP_InputField inputField;
        private UnityEngine.UI.Slider slider;
        private UnityEngine.UI.LayoutElement inputFieldLayoutElement;
        private UnityEngine.UI.HorizontalLayoutGroup textInputPanel;

        private void Start()
        {
            GetComponent<GUILabelLogic>().SetLabelData(nameLocalizationKey, unitName, prefix);

            parentSubwindow         = GUISubwindowLogic.FindParentSubWindow(gameObject);
            var contentObject       = GUISubwindowLogic.FindChildObjectByNameRecursive(gameObject, "Content").gameObject;
            slider                  = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
            textInputPanel          = GUISubwindowLogic.FindChildObjectByNameRecursive(contentObject, "TextInputPanel").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();

            inputFieldLayoutElement.minWidth = parentSubwindow.minimumInputFieldWidth;
            textInputPanel.childForceExpandWidth = !sliderEnabled;

            if (clampToMinMax)
            {
                value = Mathf.Clamp(value, minValue, maxValue);
            }

            UpdateSliderValue();
            UpdateTextFieldValue();

            // Configure slider
            slider.interactable = isInteractable;
            slider.gameObject.SetActive(sliderEnabled);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float newValue) =>
            {
                float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
                newValue = newValue * prefixFactor;
                if (newValue == value) return;

                value = newValue;
                UpdateTextFieldValue();
                OnValueChanged.Invoke(value);
            });

            // Configure textfield
            inputField.readOnly = !isInteractable;
            inputField.gameObject.SetActive(textInputEnabled);
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener((string text) =>
            {
                float newValue = value;
                bool parseSuccess = float.TryParse(text, out newValue);
                if (!parseSuccess)
                {
                    UpdateTextFieldValue(); // Restores previous value
                    return;
                }

                float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
                newValue = newValue * prefixFactor;

                if (clampToMinMax)
                {
                    newValue = Mathf.Clamp(newValue, minValue, maxValue);
                }
                if (value != newValue)
                {
                    value = newValue;
                    UpdateSliderValue();
                    UpdateTextFieldValue(); // For formating
                    OnValueChanged.Invoke(newValue);
                }
            });
        }
        private void UpdateTextFieldValue()
        {
            if (inputField == null) return;

            // There surely is a better way to specify float precision in toString...
            string formatString = "0";
            if (postCommaDigits > 0)
            {
                formatString += ".";
                for (int i = 0; i < postCommaDigits; i++)
                {
                    formatString += "0";
                }
            }
            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            inputField.text = (value / prefixFactor).ToString(formatString);
        }

        private void UpdateSliderValue()
        {
            float valuePrev = value;
            if (slider == null) return;

            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            slider.minValue = minValue / prefixFactor;
            slider.maxValue = maxValue / prefixFactor;
            slider.value = value / prefixFactor;
            value = valuePrev; // Slider may apply clamping
        }


        public float GetValue() { return value; }

        public void SetValue(float newValue)
        {
            if (clampToMinMax)
            {
                newValue = Mathf.Clamp(newValue, minValue, maxValue);
            }
            if (value != newValue)
            {
                value = newValue;
                UpdateSliderValue();
                UpdateTextFieldValue();
                OnValueChanged.Invoke(newValue);
            }
        }

        public void SetMinMax(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            if (clampToMinMax)
            {
                value = Mathf.Clamp(value, minValue, maxValue);
            }
            UpdateSliderValue();
            UpdateTextFieldValue();
            OnValueChanged.Invoke(value);
        }
    }
}
