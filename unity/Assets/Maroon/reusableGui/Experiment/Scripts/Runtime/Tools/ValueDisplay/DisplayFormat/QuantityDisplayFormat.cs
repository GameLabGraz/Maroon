using System;
using System.Globalization;
using UnityEngine;

namespace Maroon.UI.ValueDisplay
{
    [RequireComponent(typeof(QuantityValueDisplay))]
    public class QuantityDisplayFormat : MonoBehaviour, IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case int intValue:
                    return FormatInt(intValue);
                case float floatValue:
                    return FormatFloat(floatValue);
                case Vector2 vector2:
                    return FormatVector2(vector2);
                case Vector3 vector3:
                    return FormatVector3(vector3);
                default:
                    return arg.ToString();
            }
        }

        protected virtual string FormatInt(int value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture);
        }
        protected virtual string FormatFloat(float value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }

        protected virtual string FormatVector2(Vector2 value)
        {
            return $"({value.x}, {value.y})";
        }

        protected virtual string FormatVector3(Vector3 value)
        {
            return $"({value.x}, {value.y}, {value.z})";
        }
    }
}
