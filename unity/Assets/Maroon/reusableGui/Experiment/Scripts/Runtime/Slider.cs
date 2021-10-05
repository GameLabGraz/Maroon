using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Maroon.UI
{
    [Serializable]
    public class SliderInitEvent : UnityEvent<float> { }

    public class Slider : UnityEngine.UI.Slider, IResetObject
    {
        [SerializeField] private bool allowReset = true;

        [SerializeField] private SliderInitEvent onSliderInit;
        [SerializeField] private UnityEvent onStartDrag;
        [SerializeField] private UnityEvent onEndDrag;
        [SerializeField] private UnityEvent onSetSliderValueViaInput;

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
                onSetSliderValueViaInput?.Invoke();
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogException(e);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onStartDrag?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onEndDrag?.Invoke();
        }

        public void ResetObject()
        {
            if (allowReset)
                value = _startValue;
        }
    }
}
