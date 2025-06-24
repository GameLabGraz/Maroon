using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    // Note(MartinR): This is currently just a copy-paste from GuiFloatInputHandler, with float changed to int.
    //      Not sure if this could be done better, but since both of these classes shouldn't change a lot (In the best case these never change),
    //      some code-duplication here should be fine
    [ExecuteAlways]
    [RequireComponent(typeof(GuiGenericValueHandler))]
    public class GuiIntInputHandler : MonoBehaviour
    {
        [SerializeField] private int  initialValue = 1;
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool useMinMax = true;
        [SerializeField] private int  minValue = 1;
        [SerializeField] private int  maxValue = 10;

        [SerializeField] private bool sliderEnabled = true;
        [SerializeField] private bool textInputEnabled = true;

        private int value = 1;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

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
            var inputPrefab = Resources.Load("GuiIntInputPrefab");
            var inputObject = (GameObject) GameObject.Instantiate(inputPrefab, contentPanel.transform);

            // Find ui elements by name
            parentSubwindow         = GuiSubWindowHandler.FindParentSubWindow(gameObject);
            slider                  = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
            sliderPanel             = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "SliderPanel").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>(); 
            textInputPanel          = GuiSubWindowHandler.FindChildObjectByNameRecursive(contentPanel, "TextInputPanel").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>(); 

            if (slider == null || inputField == null || inputFieldLayoutElement == null)
            {
                Debug.LogWarning("GuiIntInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }
        }

        private void Start()
        {
            // Register UI-Callbacks
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float newValue) =>
            {
                SetValue((int)(newValue + 0.5f));
                OnValueChanged.Invoke(value);
            });

            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener((string text) =>
            {
                int newValue = value;
                bool parseSuccess = int.TryParse(text, out newValue);
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



        public int GetValue() { return value; }

        public void SetValue(int newValue)
        {
            if (useMinMax)
            {
                newValue = newValue < minValue ? minValue : newValue;
                newValue = newValue > maxValue ? maxValue : newValue;
            }
            value = newValue;

            // This null check is required so SetValue can be called from other gameobjects in Awake/Start
            if (inputField == null || slider == null || parentSubwindow == null) { return; }

            inputField.text = value.ToString();
            if ((int)(slider.value + 0.5f) != newValue)
            {
                slider.value = newValue;
            }
        }

        // Note: See notes in GuiFloatInputHandler.SetInitialValue
        public void SetInitialValue(int initialValue)
        {
            if (Application.isEditor && !Application.isPlaying) return;
            this.initialValue = initialValue;
        }

        public void SetMinMax(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            SetValue(value); // Clamps value to new min/max and updates text if necessary

            // Note(MartinR): This check is required so SetMinMax can be called from other gameobjects in Awake/Start
            if (inputField == null || slider == null) return;

            // Update UI slider to new min/max
            slider.minValue = minValue;
            slider.maxValue = maxValue;
        }
    }
}
