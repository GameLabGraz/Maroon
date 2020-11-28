using System.Globalization;
using Maroon.Physics;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Maroon.UI
{

    [CustomEditor(typeof(QuantityPropertyView))]
    public class QuantityPropertyViewEditor : Editor
    {
        private QuantityPropertyView _propertyView;

        private void OnEnable()
        {
            _propertyView = (QuantityPropertyView) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                _propertyView.ShowUI();
            }
        }
    }


    public class QuantityPropertyView : MonoBehaviour
    {
        [SerializeField] public QuantityReferenceValue quantity;

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

            var label = (GameObject)Instantiate(UserInterfaceContent.TextPrefab, transform);
            label.GetComponent<TextMeshProUGUI>().text = quantity.Value.GetName();

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
                
                case QuantityVector3 vectorQuantity:

                    var xInputField = ((GameObject)Instantiate(UserInterfaceContent.InputPrefab, transform)).GetComponent<TMP_InputField>();
                    xInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    xInputField.text = vectorQuantity.Value.x.ToString(CultureInfo.InvariantCulture);
                    xInputField.onEndEdit.AddListener(value =>
                    {
                        var vector = vectorQuantity.Value;
                        vector.x = float.Parse(value, CultureInfo.InvariantCulture);
                        vectorQuantity.Value = vector;
                    });

                    var yInputField = ((GameObject) Instantiate(UserInterfaceContent.InputPrefab, transform)).GetComponent<TMP_InputField>();
                    yInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    yInputField.text = vectorQuantity.Value.y.ToString(CultureInfo.InvariantCulture);
                    yInputField.onEndEdit.AddListener(value =>
                    {
                        var vector = vectorQuantity.Value;
                        vector.y = float.Parse(value, CultureInfo.InvariantCulture);
                        vectorQuantity.Value = vector;
                    });

                    var zInputField = ((GameObject)Instantiate(UserInterfaceContent.InputPrefab, transform)).GetComponent<TMP_InputField>();
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

                     break;

                case QuantityBool boolQuantity:
                    Instantiate(UserInterfaceContent.TogglePrefab, transform);
                    break;
                case QuantityString stringQuantity:
                    break;
                default:
                    Debug.LogWarning("QuantityPropertyView::LoadUI:Unsupported Quantity Type!");
                    return;
            }
        }

        private Slider InstantiateSlider(float minValue, float maxValue, Transform parent, bool wholeNumbers = false)
        {
            var slider = ((GameObject)Instantiate(UserInterfaceContent.SliderPrefab, parent))?.GetComponentInChildren<Slider>();
            if (slider == null) return null;

            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.wholeNumbers = wholeNumbers;
            return slider;
        }

    }
}
