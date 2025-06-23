using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    // Note(MartinR): This is currently just a copy-paste from GuiIntInputHandler, with int changed to float.
    //      Not sure if this could be done better, but since both of these classes shouldn't change a lot (In the best case these never change),
    //      some code-duplication here should be fine
    [ExecuteAlways]
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiFloatInputHandler : MonoBehaviour
    {
        [SerializeField] private float initialValue = 1;
        [SerializeField] private bool  isInteractable = true;
        [SerializeField] private int postCommaDigits = 2;
        [SerializeField] private bool  useMinMax = true;
        [SerializeField] private float minValue = 1;
        [SerializeField] private float maxValue = 10;

        [SerializeField] private bool sliderEnabled = true;
        [SerializeField] private bool textInputEnabled = true;

        private float value = 1;
        private bool ignoreNextSliderInput = false;

        public UnityEngine.Events.UnityEvent<float> OnValueChanged;

        private GuiSubWindowHandler parentSubwindow;
        private TMPro.TMP_InputField inputField;
        private UnityEngine.UI.Slider slider;
        private UnityEngine.UI.LayoutElement inputFieldLayoutElement;
        private UnityEngine.UI.HorizontalLayoutGroup sliderPanel;
        private UnityEngine.UI.HorizontalLayoutGroup textInputPanel;

        private void Awake()
        {
            // Create UI elements by instanciating prefab
            var contentPanel = GetComponent<GuiGenericValueHandler>().GetEmptyContentPanel();
            var inputPrefab = Resources.Load("GuiFloatInputPrefab");
            var inputObject = (GameObject) GameObject.Instantiate(inputPrefab, contentPanel.transform);

            // Find ui elements by name reference gameobjects by name
            parentSubwindow         = GuiSubWindowHandler.FindParentSubWindow(gameObject);
            slider                  = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
            sliderPanel             = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "SliderPanel").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>(); 
            textInputPanel          = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "TextInputPanel").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>(); 

            if (slider == null || inputField == null || inputFieldLayoutElement == null)
            {
                Debug.LogWarning("GuiFloatInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }
        }

        private void Start()
        {
            // Register UI-Callbacks
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float newValue) =>
            {
                if (ignoreNextSliderInput)
                {
                    ignoreNextSliderInput = false;
                    return;
                }

                SetValue(newValue);
                OnValueChanged.Invoke(value);
            });

            inputField.onEndEdit.RemoveAllListeners();
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

            SetUIElementValues();
            SetValue(initialValue);
        }

        private void SetUIElementValues()
        {
            if (inputField == null || slider == null || parentSubwindow == null) return;

            // Initialize ui elements
            slider.interactable = isInteractable;
            inputField.readOnly = !isInteractable;

            sliderPanel.gameObject.SetActive(sliderEnabled);
            textInputPanel.gameObject.SetActive(textInputEnabled);
            textInputPanel.childForceExpandWidth = !sliderEnabled; // Expand text field to whole size if no slider is present

            SetMinMax(minValue, maxValue);
            SetValue(value); // Applies min and max to initial value, also sets slider and input field text

            inputFieldLayoutElement.minWidth = parentSubwindow.minimumInputFieldWidth;
        }

        private void OnValidate() { SetUIElementValues(); SetValue(initialValue); }



        public float GetValue() { return value; }

        public void SetValue(float newValue)
        {
            if (useMinMax)
            {
                newValue = newValue < minValue ? minValue : newValue;
                newValue = newValue > maxValue ? maxValue : newValue;
            }
            value = newValue;

            if (!Application.isEditor || Application.isPlaying)
            {
                initialValue = newValue; // This is required if SetValue is used before Start() was called on this object
            }

            string formatString = "0";
            if (postCommaDigits > 0)
            {
                formatString += ".";
                for (int i = 0; i < postCommaDigits; i++)
                {
                    formatString += "0";
                }
            }

            // Note(MartinR): This check is required so SetValue can be called from other gameobjects in Awake
            if (inputField == null || slider == null) return;

            inputField.text = value.ToString(formatString);
            ignoreNextSliderInput = true;
            slider.value = value;
        }

        public void SetMinMax(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            SetValue(value); // Clamps value to new min/max and updates text if necessary

            // Note(MartinR): This check is required so SetMinMax can be called from other gameobjects in Awake
            if (inputField == null || slider == null) return;

            // Update UI slider to new min/max
            // Note: Slider always uses min/max value, even if useMinMax is false
            slider.minValue = minValue;
            slider.maxValue = maxValue;
        }
    }
}
