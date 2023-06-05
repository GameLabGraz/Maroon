using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ResetToggle : ResetUI
    {
        protected Toggle _toggle;
        protected bool _startValue;

        protected void Start()
        {
            _toggle = GetComponent<Toggle>();
            _startValue = _toggle.isOn;
        }

        public override void ResetObject()
        {
            _toggle.isOn = _startValue;
        }
    }
}
