using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.UI
{
    [Serializable]
    public class SliderInitEvent : UnityEvent<float> { }

    public class Slider : UnityEngine.UI.Slider, IResetObject
    {
        [SerializeField] private bool allowReset = true;

        [SerializeField] private SliderInitEvent onSliderInit;

        private float _startValue;

        public bool AllowReset
        {
            get => allowReset;
            set => allowReset = value;
        }

        protected override void Start()
        {
            base.Start();

            _startValue = value;
            onSliderInit?.Invoke(_startValue);
        }

        public void SetSliderValue(object valueObject)
        {
            try
            {
                value = (float)Convert.ToDouble(valueObject);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void ResetObject()
        {
            if (allowReset)
                value = _startValue;
        }
    }
}
