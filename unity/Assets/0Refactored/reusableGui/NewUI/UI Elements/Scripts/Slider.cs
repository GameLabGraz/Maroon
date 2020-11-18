using System;
using UnityEngine;

namespace Maroon.UI
{
    public class Slider : UnityEngine.UI.Slider, IResetObject
    {
        private float _startValue;
        public bool resetEnabled = true;

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
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void ResetObject()
        {
            if (resetEnabled)
                value = _startValue;
        }
    }
}
