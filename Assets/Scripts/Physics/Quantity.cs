using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Physics
{
    public class OnValueChangeEvent<T> : UnityEvent<T> {}
    [Serializable]
    public class OnValueFloatChangeEvent : OnValueChangeEvent<float> { }
    [Serializable]
    public class OnValueVector3ChangeEvent : OnValueChangeEvent<Vector3> { }

    public interface IQuantity
    {
        object GetValue();
    }

    [Serializable]
    public class Quantity<T>: IQuantity
    {
        [SerializeField]
        private T _value;
        public T Value
        {
            get => _value;
            set
            {

                _value = value;
                onValueChanged?.Invoke(value);
            }
        }

        public Quantity(T value) { _value = value; }

        public OnValueChangeEvent<T> onValueChanged = new OnValueChangeEvent<T>();

        public object GetValue()
        {
            return _value;
        }
    }

    [Serializable]
    public class QuantityFloat : Quantity<float>
    {
        public QuantityFloat(float value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler); }
        public new OnValueFloatChangeEvent onValueChanged = new OnValueFloatChangeEvent();
        private void OnValueChangedHandler(float value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityFloat(float value) => new QuantityFloat(value);
        public static implicit operator float(QuantityFloat quantity) => quantity.Value;
    }

    [Serializable]
    public class QuantityVector3 : Quantity<Vector3>
    {
        public QuantityVector3(Vector3 value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler); }
        public new OnValueVector3ChangeEvent onValueChanged = new OnValueVector3ChangeEvent();
        private void OnValueChangedHandler(Vector3 value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityVector3(Vector3 value) => new QuantityVector3(value);
        public static implicit operator Vector3(QuantityVector3 quantity) => quantity.Value;
    }
}