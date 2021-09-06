using TMPro;
using UnityEngine;

namespace Maroon.UI
{
    public class InputField : TMP_InputField, IResetObject
    {
        [SerializeField] private bool allowReset = true;

        private string _startText;

        public bool AllowReset
        {
            get => allowReset;
            set => allowReset = value;
        }

        protected override void Start()
        {
            _startText = text;
        }

        public void SetText(float value)
        {
            text = $"{value:0.###}";
        }

        public void ResetObject()
        {
            if (allowReset)
                text = _startText;
        }
    }
}
