using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

namespace Maroon.Physics
{
    public class OnValueChangeEvent<T> : UnityEvent<T> {}
    [Serializable]
    public class OnValueChangeEventParam<T> : UnityEvent<T, string, bool> { }
    [Serializable]
    public class OnValueChangeEventParamFloat : OnValueChangeEventParam<float> { }
    [Serializable]
    public class OnValueChangeEventParamVec3 : OnValueChangeEventParam<Vector3> { }
    [Serializable]
    public class OnValueChangeEventParamBool : OnValueChangeEventParam<bool> { }
    [Serializable]
    public class OnValueChangeEventParamInt : OnValueChangeEventParam<int> { }
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
        bool IsDynamic();
        object GetValue();

        void SendValueChangedEvent();
    }
    
    
    [Serializable]
    public class Quantity<T>: IQuantity
    {
        [SerializeField] public string assessmentName;
        public bool isDynamic = false;

        public bool alwaysSendValueChangedEvent = true;

        [SerializeField]
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                if(alwaysSendValueChangedEvent)
                    onValueChanged?.Invoke(value);
            }
        }

        public Quantity(T value)
        {
            _value = value;
        }

        public OnValueChangeEvent<T> onValueChanged = new OnValueChangeEvent<T>();

        public string GetName()
        {
            return assessmentName;
        }

        public bool IsDynamic()
        {
            return isDynamic;
        }

        public object GetValue()
        {
            return _value;
        }

        public void SendValueChangedEvent()
        {
            onValueChanged.Invoke(_value);
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
        public QuantityVector3(Vector3 value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler);}
        public new OnValueVector3ChangeEvent onValueChanged = new OnValueVector3ChangeEvent();
        private void OnValueChangedHandler(Vector3 value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityVector3(Vector3 value) => new QuantityVector3(value);
        public static implicit operator Vector3(QuantityVector3 quantity) => quantity.Value;
    }
    
    [Serializable]
    public class QuantityBool : Quantity<bool>
    {
        public QuantityBool(bool value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler); }
        public new OnValueBoolChangeEvent onValueChanged = new OnValueBoolChangeEvent();
        private void OnValueChangedHandler(bool value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityBool(bool value) => new QuantityBool(value);
        public static implicit operator bool(QuantityBool quantity) => quantity.Value;
    }
    
    [Serializable]
    public class QuantityInt : Quantity<int>
    {
        public QuantityInt(int value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler); }
        public new OnValueIntChangeEvent onValueChanged = new OnValueIntChangeEvent();
        private void OnValueChangedHandler(int value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityInt(int value) => new QuantityInt(value);
        public static implicit operator int(QuantityInt quantity) => quantity.Value;
    }
    
    [Serializable]
    public class QuantityString : Quantity<string>
    {
        public QuantityString(string value) : base(value) { base.onValueChanged.AddListener(OnValueChangedHandler); }
        public new OnValueStringChangeEvent onValueChanged = new OnValueStringChangeEvent();
        private void OnValueChangedHandler(string value) { onValueChanged?.Invoke(value); }
        public static implicit operator QuantityString(string value) => new QuantityString(value);
        public static implicit operator string(QuantityString quantity) => quantity.Value;
    }
}