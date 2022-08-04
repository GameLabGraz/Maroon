using GameLabGraz.UI;
using UnityEngine;

namespace Maroon.UI
{
    [RequireComponent(typeof(InputField))]
    public class ResetInputField : ResetUI
    {
        protected InputField _inputField;
        protected string _startValue;

        protected void Start()
        {
            _inputField = GetComponent<InputField>();
            _startValue = _inputField.text;
        }

        public override void ResetObject()
        {
            if (AllowReset)
                _inputField.text = _startValue;
        }
    }
}
