using System.Globalization;
using GEAR.Localization;
using Maroon.Physics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using GameLabGraz.UI;

namespace Maroon.UI
{
    public class QuantityPropertyView : MonoBehaviour
    {
        public QuantityReferenceValue quantity;

        public UnityEvent OnQuantityViewVisible;

        private void Start()
        {
            ShowUI();
        }

        public void ClearUI()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        
        public void ShowUI()
        {
            ClearUI();

            if (quantity?.Value == null) return;

            var label = InstantiateLabel(quantity.Value.GetName());

            switch (quantity.Value)
            {
                case QuantityInt intQuantity:
                    var sliderInt = InstantiateSlider(
                        intQuantity.minValue, 
                        intQuantity.maxValue, 
                        transform,
                        true);
                    sliderInt.value = intQuantity.Value;

                    sliderInt.onValueChanged.AddListener(value =>
                    {
                        intQuantity.Value = (int)value;
                    });
                
                    intQuantity.onValueChanged.AddListener(value => { sliderInt.value = value; });

                    break;
                
                case QuantityFloat floatQuantity:
                    var sliderFloat = InstantiateSlider(
                        floatQuantity.minValue,
                        floatQuantity.maxValue,
                        transform);
                    sliderFloat.value = floatQuantity.Value;

                    sliderFloat.onValueChanged.AddListener(value =>
                    {
                        floatQuantity.Value = value;
                    });

                    floatQuantity.onValueChanged.AddListener(value => { sliderFloat.value = value; });
                    break;
                
                case QuantityDecimal decimalQuantity:
                    var sliderDecimal = InstantiateSlider(
                        (float)decimalQuantity.minValue,
                        (float)decimalQuantity.maxValue,
                        transform);
                    sliderDecimal.value = (float)decimalQuantity.Value;

                    sliderDecimal.onValueChanged.AddListener(value =>
                    {
                        decimalQuantity.Value = (decimal)value;
                    });

                    decimalQuantity.onValueChanged.AddListener(value => { sliderDecimal.value = (float)value; });
                    break;

                case QuantityVector3 vectorQuantity:

                    var xInputField = ((GameObject)Instantiate(UIContent.Input, transform)).GetComponent<TMP_InputField>();
                    xInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    xInputField.text = vectorQuantity.Value.x.ToString(CultureInfo.InvariantCulture);
                    xInputField.onEndEdit.AddListener(value =>
                    {
                        var vector = vectorQuantity.Value;
                        vector.x = float.Parse(value, CultureInfo.InvariantCulture);
                        vectorQuantity.Value = vector;
                    });

                    var yInputField = ((GameObject) Instantiate(UIContent.Input, transform)).GetComponent<TMP_InputField>();
                    yInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    yInputField.text = vectorQuantity.Value.y.ToString(CultureInfo.InvariantCulture);
                    yInputField.onEndEdit.AddListener(value =>
                    {
                        var vector = vectorQuantity.Value;
                        vector.y = float.Parse(value, CultureInfo.InvariantCulture);
                        vectorQuantity.Value = vector;
                    });

                    var zInputField = ((GameObject)Instantiate(UIContent.Input, transform)).GetComponent<TMP_InputField>();
                    zInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    zInputField.text = vectorQuantity.Value.z.ToString(CultureInfo.InvariantCulture);
                    zInputField.onEndEdit.AddListener(value =>
                    {
                        var vector = vectorQuantity.Value;
                        vector.z = float.Parse(value, CultureInfo.InvariantCulture);
                        vectorQuantity.Value = vector;
                    });

                    vectorQuantity.onValueChanged.AddListener(value =>
                    {
                        xInputField.text = value.x.ToString(CultureInfo.InvariantCulture);
                        yInputField.text = value.y.ToString(CultureInfo.InvariantCulture);
                        zInputField.text = value.z.ToString(CultureInfo.InvariantCulture);
                    });

                    //ToDo: Add a reset script to vector quantity.
                    break;

                case QuantityBool boolQuantity:
                    var toggle = (GameObject)Instantiate(UIContent.ToggleGroup, transform);
                    toggle.AddComponent<ResetToggle>();
                    break;
                case QuantityString stringQuantity:
                    break;
                default:
                    Debug.LogWarning("QuantityPropertyView::LoadUI:Unsupported Quantity Type!");
                    return;
            }
            OnQuantityViewVisible?.Invoke();
        }

        private TextMeshProUGUI InstantiateLabel(string text)
        {
            var label = ((GameObject)Instantiate(UIContent.Text, transform))?.GetComponent<TextMeshProUGUI>();
            if (label == null) return null;

            if (LanguageManager.Instance)
            {
                label.text = LanguageManager.Instance.GetString(text);
                LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
                {
                    label.text = LanguageManager.Instance.GetString(text);
                });
            }
            else
            {
                label.text = text;
            }
            return label;
        }

        private Slider InstantiateSlider(float minValue, float maxValue, Transform parent, bool wholeNumbers = false)
        {
            var slider = ((GameObject)Instantiate(UIContent.SliderPrefab, parent))?.GetComponentInChildren<Slider>();
            if (slider == null) return null;

            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.wholeNumbers = wholeNumbers;
            
            return slider;
        }

    }
}
