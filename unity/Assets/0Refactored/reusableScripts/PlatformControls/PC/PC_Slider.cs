using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlatformControls.PC
{
    public class PC_Slider : Slider, IResetObject
    {
        private float _startValue;
        public bool resetEnabled = true;

        public UnityEvent onStartDrag = new UnityEvent();
        public UnityEvent onEndDrag = new UnityEvent();
        public UnityEvent onSetSliderValueViaInput = new UnityEvent();

        protected override void Start()
        {
            base.Start();
            _startValue = value;
        }
        
        public void SetSliderValue(object valueObject)
        {
            try
            {
                value = (float)Convert.ToDouble(valueObject);
                onSetSliderValueViaInput.Invoke();
            }
            catch (Exception e)
            {
                value = 0;
                Debug.LogException(e);
            }         
        }

        public void ResetObject()
        {
            if(resetEnabled)
                value = _startValue;
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onStartDrag.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onEndDrag.Invoke();
        }
    }
}
