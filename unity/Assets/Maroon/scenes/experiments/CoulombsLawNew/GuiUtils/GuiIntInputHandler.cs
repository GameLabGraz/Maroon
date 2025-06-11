using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    // Note(MartinR): This is currently just a copy-paste from GuiFloatInputHandler, with float changed to int.
    //      Not sure if this could be done better, but since both of these classes shouldn't change a lot (In the best case these never change),
    //      some code-duplication here should be fine
    public class GuiIntInputHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name:";
        [SerializeField] private string unitName = "m";
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 10;
        [SerializeField] private int value = 1;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

        // Private references to UI-Elements
        // Note(MartinR): In Awake these references are searched by name instead of having references set via [SerializeField],
        //      because I wanted these fields to not show up in the unity editor when using the prefab
        private TMPro.TMP_Text nameLabel;
        private TMPro.TMP_Text unitLabel;
        private TMPro.TMP_InputField inputField;
        private UnityEngine.UI.Slider slider;
        private UnityEngine.UI.LayoutElement inputFieldLayoutElement;

        private int MaxInt(int a, int b) { return a > b ? a : b; }

        // Note(MartinR): Changing properties through the Unity-Inspector should immediately show up in the Unity-Editor,
        //      and I implemented this by using OnValidate (Not sure if there are better ways to do this, like [AlwaysExecute])
        private void Initialize()
        {
            // Find ui elements by name reference gameobjects by name
            nameLabel               = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueName").GetComponent<TMPro.TMP_Text>();
            unitLabel               = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "UnitLabel").GetComponent<TMPro.TMP_Text>();
            slider                  = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
            if (nameLabel == null || unitLabel == null || slider == null || inputField == null || inputFieldLayoutElement == null)
            {
                Debug.LogWarning("GuiFloatInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }

            // Initialize ui elements
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            SetValue(value); // Applies min and max to initial value, also sets slider and input field text

            const float APPROXIMATE_CHAR_WIDTH = 28; // Note(MartinR): Just an approximation with font size 36
            float maxAbsValue = Mathf.Max(Mathf.Abs(minValue), Mathf.Abs(maxValue));
            int expectedPreCommaDigits = MaxInt(1, 1 + (int) (Mathf.Log10(maxAbsValue) + 0.5f));
            // Text-Field minWidth depends on all characters, including the dot "." and a possible "-" at the start
            inputFieldLayoutElement.minWidth = APPROXIMATE_CHAR_WIDTH * (expectedPreCommaDigits + (minValue < 0 ? 1 : 0));

            nameLabel.text = valueName;
            unitLabel.text = unitName;
            unitLabel.gameObject.SetActive(unitName.Length > 0);

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
        }

        private void Awake() { Initialize(); }

        private void OnValidate() { Initialize(); }

        public int GetValue() { return value; }

        public void SetValue(int newValue)
        {
            newValue = newValue < minValue ? minValue : newValue;
            newValue = newValue > maxValue ? maxValue : newValue;
            value = newValue;

            inputField.text = value.ToString();
            if ((int)(slider.value + 0.5f) != newValue)
            {
                slider.value = newValue;
            }
        }
    }
}
