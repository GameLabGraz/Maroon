using TMPro;
using UnityEngine;

namespace Maroon.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ResetDropDown : ResetUI
    {
        protected TMP_Dropdown _dropdown;
        protected int _startValue;

        protected void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _startValue = _dropdown.value;
        }

        public override void ResetObject()
        {
            if (allowReset)
                _dropdown.value = _startValue;
        }
    }
}

