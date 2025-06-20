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
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 10;
        [SerializeField] private int value = 1;

        [SerializeField] private bool sliderEnabled = true;
        [SerializeField] private bool inputFieldEnabled = true;

        public UnityEngine.Events.UnityEvent<int> OnValueChanged;

        // Private references to UI-Elements
        // Note(MartinR): In Awake these references are searched by name instead of having references set via [SerializeField],
        //      because I wanted these fields to not show up in the unity editor when using the prefab
        private TMPro.TMP_InputField inputField;
        private UnityEngine.UI.Slider slider;
        private UnityEngine.UI.LayoutElement inputFieldLayoutElement;

        private int MaxInt(int a, int b) { return a > b ? a : b; }

        // Note(MartinR): Changing properties through the Unity-Inspector should immediately show up in the Unity-Editor,
        //      and I implemented this by using OnValidate (Not sure if there are better ways to do this, like [AlwaysExecute])
        private void Initialize()
        {
            // Find ui elements by name reference gameobjects by name
            slider                  = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "Slider").GetComponent<UnityEngine.UI.Slider>();
            inputField              = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<TMPro.TMP_InputField>();
            inputFieldLayoutElement = GuiFloatInputHandler.FindChildObjectByNameRecursive(gameObject, "ValueInputField").GetComponent<UnityEngine.UI.LayoutElement>();
            if (slider == null || inputField == null || inputFieldLayoutElement == null)
            {
                Debug.LogWarning("GuiFloatInputHandler should be able to find all child objects if the prefab is used correctly");
                return;
            }

            // Initialize ui elements
            inputField.gameObject.SetActive(inputFieldEnabled);
            slider.gameObject.SetActive(sliderEnabled);

            slider.minValue = minValue;
            slider.maxValue = maxValue;
            SetValue(value); // Applies min and max to initial value, also sets slider and input field text

            inputFieldLayoutElement.minWidth = GuiFloatInputHandler.FindParentSubWindow(gameObject).minimumInputFieldWidth;

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

            // Note(MartinR): This check is required so SetValue can be called from other gameobjects in Awake
            if (inputField == null || slider == null) return;

            inputField.text = value.ToString();
            if ((int)(slider.value + 0.5f) != newValue)
            {
                slider.value = newValue;
            }
        }
    }
}
