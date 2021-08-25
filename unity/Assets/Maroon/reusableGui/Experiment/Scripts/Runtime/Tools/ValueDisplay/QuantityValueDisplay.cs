using Maroon.Physics;
using UnityEngine;

namespace Maroon.UI.ValueDisplay
{
    public abstract class QuantityValueDisplay : MonoBehaviour
    {
        [SerializeField] protected QuantityReferenceValue valueReference;

        [SerializeField] protected string unit;

        [SerializeField] protected QuantityDisplayFormat displayFormat;

        protected void Start()
        {
            switch (valueReference.Value)
            {
                case QuantityInt quantityInt:
                    quantityInt.onValueChanged.AddListener(intValue =>
                    {
                        UpdateDisplay(string.Format(displayFormat, "{0} {1}", intValue, unit));
                    });
                    break;
                case QuantityFloat quantityFloat:
                    quantityFloat.onValueChanged.AddListener(floatValue =>
                    {
                        UpdateDisplay(string.Format(displayFormat, "{0} {1}", floatValue, unit));
                    });
                    break;
                case QuantityBool quantityBool:
                    quantityBool.onValueChanged.AddListener(boolValue =>
                    {
                        UpdateDisplay(string.Format(displayFormat, "{0} {1}", boolValue, unit));
                    });
                    break;
                case QuantityString quantityString:
                    quantityString.onValueChanged.AddListener(stringValue =>
                    {
                        UpdateDisplay(string.Format(displayFormat, "{0} {1}", stringValue, unit));
                    });
                    break;
                case QuantityVector3 quantityVector3:
                    quantityVector3.onValueChanged.AddListener(vector3Value =>
                    {
                        UpdateDisplay(string.Format(displayFormat, "{0} {1}", vector3Value, unit));
                    });
                    break;
            }
        }

        protected abstract void UpdateDisplay(string value);
    }
}
