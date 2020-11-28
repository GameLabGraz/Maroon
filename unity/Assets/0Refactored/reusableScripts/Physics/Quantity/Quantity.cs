using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Physics
{
    public class OnValueChangeEvent<T> : UnityEvent<T> { }

    [Serializable]
    public class OnValueFloatChangeEvent : OnValueChangeEvent<float> { }
    [Serializable]
    public class OnValueVector3ChangeEvent : OnValueChangeEvent<Vector3> { }
    [Serializable]
    public class OnValueBoolChangeEvent : OnValueChangeEvent<bool> { }
    [Serializable]
    public class OnValueIntChangeEvent : OnValueChangeEvent<int> { }
    [Serializable]
    public class OnValueStringChangeEvent : OnValueChangeEvent<string> { }

    public interface IQuantity
    {
        string GetName();
        object GetValue();
    }


    [Serializable]
    public class Quantity<T> : IQuantity
    {
        [SerializeField] private string name;

        [SerializeField] private T _value;


        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                onValueChanged?.Invoke(value);
            }
        }

        public Quantity(T value)
        {
            _value = value;
        }

        public void SystemSetsQuantity(object newValue)
        {
            if (newValue.GetType() != typeof(T)) return;

            _value = (T)newValue;
            onValueChanged?.Invoke(_value);
        }

        public OnValueChangeEvent<T> onValueChanged = new OnValueChangeEvent<T>();

        public string GetName()
        {
            return name;
        }

        public object GetValue()
        {
            return _value;
        }
    }

    [Serializable]
    public class QuantityFloat : Quantity<float>
    {
        public float minValue;
        public float maxValue;

        public QuantityFloat() : this(0) { }
        public QuantityFloat(float value) : base(value)
        {
            base.onValueChanged.AddListener(OnValueChangedHandler);
        }
        public new OnValueFloatChangeEvent onValueChanged = new OnValueFloatChangeEvent();
        private void OnValueChangedHandler(float value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityFloat(float value) => new QuantityFloat(value);
        public static implicit operator float(QuantityFloat quantity) => quantity.Value;
    }

    [Serializable]
    public class QuantityVector3 : Quantity<Vector3>
    {
        public QuantityVector3() : this(Vector3.zero) { }
        public QuantityVector3(Vector3 value) : base(value)
        {
            base.onValueChanged.AddListener(OnValueChangedHandler);
        }
        public new OnValueVector3ChangeEvent onValueChanged = new OnValueVector3ChangeEvent();
        private void OnValueChangedHandler(Vector3 value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityVector3(Vector3 value) => new QuantityVector3(value);
        public static implicit operator Vector3(QuantityVector3 quantity) => quantity.Value;
    }

    [Serializable]
    public class QuantityBool : Quantity<bool>
    {
        public QuantityBool() : this(false) { }
        public QuantityBool(bool value) : base(value)
        {
            base.onValueChanged.AddListener(OnValueChangedHandler);
        }
        public new OnValueBoolChangeEvent onValueChanged = new OnValueBoolChangeEvent();
        private void OnValueChangedHandler(bool value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityBool(bool value) => new QuantityBool(value);
        public static implicit operator bool(QuantityBool quantity) => quantity.Value;
    }

    [Serializable]
    public class QuantityInt : Quantity<int>
    {
        public int minValue;
        public int maxValue;

        public QuantityInt() : this(0) { }
        public QuantityInt(int value) : base(value)
        {
            base.onValueChanged.AddListener(OnValueChangedHandler);
        }
        public new OnValueIntChangeEvent onValueChanged = new OnValueIntChangeEvent();
        private void OnValueChangedHandler(int value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityInt(int value) => new QuantityInt(value);
        public static implicit operator int(QuantityInt quantity) => quantity.Value;
    }

    [Serializable]
    public class QuantityString : Quantity<string>
    {
        public QuantityString() : this(string.Empty) { }
        public QuantityString(string value) : base(value)
        {
            base.onValueChanged.AddListener(OnValueChangedHandler);
        }
        public new OnValueStringChangeEvent onValueChanged = new OnValueStringChangeEvent();
        private void OnValueChangedHandler(string value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityString(string value) => new QuantityString(value);
        public static implicit operator string(QuantityString quantity) => quantity.Value;
    }
}