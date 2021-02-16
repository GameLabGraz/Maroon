using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.UI
{
    public class Slider : UnityEngine.UI.Slider, IResetObject
    {
        [SerializeField] private bool allowReset = true;

        public UnityEvent OnStart;

        private float _startValue;

        protected override void Start()
        {
            base.Start();

            _startValue = value;
            
            OnStart?.Invoke();
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
