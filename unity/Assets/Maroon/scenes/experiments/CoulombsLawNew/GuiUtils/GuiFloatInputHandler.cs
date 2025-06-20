using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class GuiFloatInputHandler : MonoBehaviour
    {
        [SerializeField] private string valueName = "Name:";
        [SerializeField] private string unitName = "m";
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 1;
        [SerializeField] private float value = 0.5f;
        [SerializeField] private int postCommaDigits = 2;

        public UnityEngine.Events.UnityEvent<float> OnValueChanged;

        // Private references to UI-Elements
        // Note(MartinR): In Awake these references are searched by name instead of having references set via [SerializeField],
        //      because I wanted these fields to not show up in the unity editor when using the prefab
        private TMPro.TMP_Text nameLabel;
        private TMPro.TMP_Text unitLabel;
        private TMPro.TMP_InputField inputField;
        private UnityEngine.UI.Slider slider;
        private UnityEngine.UI.LayoutElement inputFieldLayoutElement;

        public static GameObject FindChildObjectByNameRecursive(GameObject gameObject, string name)
        {
            if (gameObject.name == name) return gameObject;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var foundObject = FindChildObjectByNameRecursive(gameObject.transform.GetChild(i).gameObject, name);
                if (foundObject != null) return foundObject;
            }
            return null;
        }

        private int MaxInt(int a, int b) { return a > b ? a : b; }

        // Note(MartinR): Changing properties through the Unity-Inspector should immediately show up in the Unity-Editor,
        //      and I implemented this by using OnValidate (Not sure if there are better ways to do this, like [AlwaysExecute])
        private void Initialize()
        {
            // Find ui elements by name reference gameobjects by name
            nameLabel               = FindChildObjectByNameRecursive(gameObject, "ValueName").GetComponent<TMPro.TMP_Text>();
            unitLabel               = FindChildObjectByNameRecursive(gameObject, "UnitLabel").GetComponent<TMPro.TMP_Text>();
            slider                  = FindChildObjectByNameRecursive(gameObject, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
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
            int expectedPreCommaDigits = MaxInt(1, (int) (Mathf.Log10(maxAbsValue) + 0.5f));
            // Text-Field minWidth depends on all characters, including the dot "." and a possible "-" at the start
            inputFieldLayoutElement.minWidth = 
                APPROXIMATE_CHAR_WIDTH * (expectedPreCommaDigits + postCommaDigits + (postCommaDigits > 0 ? 1 : 0) + (minValue < 0 ? 1 : 0));

            nameLabel.text = valueName;
            unitLabel.text = unitName;
            unitLabel.gameObject.SetActive(unitName.Length > 0);

            // Register UI-Callbacks
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float newValue) =>
            {
                SetValue(newValue);
                OnValueChanged.Invoke(newValue);
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
        }

        private void Awake() { Initialize(); }

        private void OnValidate() { Initialize(); }

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

            // Note(MartinR): This check is required so SetValue can be called from other gameobjects in Awake
            if (inputField == null || slider == null) return;

            inputField.text = value.ToString(formatString);
            if (slider.value != newValue)
            {
                slider.value = newValue;
            }
        }

        public void SetMinMax(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            SetValue(value); // Clamps value to new min/max and updates text if necessary

            // Note(MartinR): This check is required so SetMinMax can be called from other gameobjects in Awake
            if (inputField == null || slider == null) return;

            // Update UI slider to new min/max
            slider.minValue = minValue;
            slider.maxValue = maxValue;
        }
    }
}
