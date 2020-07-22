using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlatformControls.PC
{
    public class PC_Slider : Slider, IResetObject
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
                value = 0;
                Debug.LogException(e);
            }         
        }

        public void ResetObject()
        {
            if(resetEnabled)
                value = _startValue;
        }
    }
}
