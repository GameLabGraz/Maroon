using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    // Note(MartinR): This is currently just a copy-paste from GUIFloatInputLogic, with float changed to int
    [RequireComponent(typeof(GUILabelLogic))]
    public class GUIIntInputLogic : MonoBehaviour
    {
        [SerializeField] private int       value = 1;
        [SerializeField] private bool      isInteractable = true;
        [SerializeField] private string    nameLocalizationKey = "";
        [SerializeField] private string    unitName = "";
        [SerializeField] private SI_Prefix prefix = SI_Prefix.NONE;
        [SerializeField] private bool      clampToMinMax = true;
        [SerializeField] private int       minValue = 1;
        [SerializeField] private int       maxValue = 10;
        [SerializeField] private bool      sliderEnabled = true;
        [SerializeField] private bool      textInputEnabled = true;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

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
                value = intClamp(value, minValue, maxValue);
            }

            UpdateSliderValue();
            UpdateTextFieldValue();

            // Configure slider
            slider.interactable = isInteractable;
            slider.gameObject.SetActive(sliderEnabled);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float newValueFloat) =>
            {
                float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
                newValueFloat = newValueFloat * prefixFactor;

                int newValue = (int)(newValueFloat + 0.5f);
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
                float newValueFloat = value;
                bool parseSuccess = float.TryParse(text, out newValueFloat);
                if (!parseSuccess)
                {
                    UpdateTextFieldValue(); // Restores previous value
                    return;
                }

                float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
                newValueFloat = newValueFloat * prefixFactor;
                int newValue = (int)(newValueFloat + 0.5f);
                if (clampToMinMax)
                {
                    newValue = intClamp(newValue, minValue, maxValue);
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
            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            inputField.text = ((float)value / prefixFactor).ToString("0");
        }

        private void UpdateSliderValue()
        {
            int valuePrev = value;
            if (slider == null) return;

            float prefixFactor = (new SI_Prefix_Info(prefix)).factor;
            slider.minValue = minValue / prefixFactor;
            slider.maxValue = maxValue / prefixFactor;
            slider.value = value / prefixFactor;
            value = valuePrev; // Slider may apply clamping
        }

        private static int intClamp(int value, int minValue, int maxValue)
        {
            if (value < minValue) return minValue;
            if (value > maxValue) return maxValue;
            return value;
        }

        public int GetValue() { return value; }

        public void SetValue(int newValue)
        {
            if (clampToMinMax)
            {
                newValue = intClamp(newValue, minValue, maxValue);
            }
            if (value != newValue)
            {
                value = newValue;
                UpdateSliderValue();
                UpdateTextFieldValue();
                OnValueChanged.Invoke(newValue);
            }
        }

        public void SetMinMax(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            if (clampToMinMax)
            {
                value = intClamp(value, minValue, maxValue);
            }
            UpdateSliderValue();
            UpdateTextFieldValue();
            OnValueChanged.Invoke(value);
        }
    }
}
