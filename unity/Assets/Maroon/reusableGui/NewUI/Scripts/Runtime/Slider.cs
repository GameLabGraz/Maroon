using System;
using UnityEngine;

namespace Maroon.UI
{
    public class Slider : UnityEngine.UI.Slider, IResetObject
    {
        [SerializeField] private bool allowReset = true;

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
