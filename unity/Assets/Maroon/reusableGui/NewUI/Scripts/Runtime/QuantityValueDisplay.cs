using System.Globalization;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.UI
{

    [RequireComponent(typeof(InputField))]
    public class QuantityValueDisplay : MonoBehaviour
    {
        [SerializeField] private QuantityReferenceValue valueReference;

        [SerializeField] private string unit;

        private void Start()
        {
            var valueDisplayField = GetComponent<InputField>();
            valueDisplayField.interactable = false; // only for displaying the value.

            switch (valueReference.Value)
            {
                case QuantityInt quantityInt:
                    quantityInt.onValueChanged.AddListener(intValue =>
                    {
                        valueDisplayField.text = $"{intValue.ToString("D", CultureInfo.InvariantCulture)} {unit}";
                    });
                    break;
                case QuantityFloat quantityFloat:
                    quantityFloat.onValueChanged.AddListener(floatValue =>
                    {
                        valueDisplayField.text = $"{floatValue.ToString(CultureInfo.InvariantCulture)} {unit}";
                    });
                    break;
                case QuantityBool quantityBool:
                    quantityBool.onValueChanged.AddListener(boolValue =>
                    {
                        valueDisplayField.text = $"{boolValue} {unit}";
                    });
                    break;
                case QuantityString quantityString:
                    quantityString.onValueChanged.AddListener(stringValue =>
                    {
                        valueDisplayField.text = $"{stringValue} {unit}";
                    });
                    break;
                case QuantityVector3 quantityVector3:
                    quantityVector3.onValueChanged.AddListener(vector3Value =>
                    {
                        valueDisplayField.text = $"({vector3Value.x}, {vector3Value.y}, {vector3Value.z}) {unit}";
                    });
                    break;
            }
        }
    }
}
