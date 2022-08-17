using GameLabGraz.UI;
using UnityEngine;

namespace Maroon.UI
{
    [RequireComponent(typeof(Slider))]
    public class ResetSlider : ResetUI
    {
        protected Slider _slider;
        protected float _startValue;

        protected void Start()
        {
            _slider = GetComponent<Slider>();
            _startValue = _slider.value;
        }

        public override void ResetObject()
        {
            if (allowReset)
                _slider.value = _startValue;
        }
    }
}
