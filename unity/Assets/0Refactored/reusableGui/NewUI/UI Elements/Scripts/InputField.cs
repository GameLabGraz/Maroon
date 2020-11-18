using System.Globalization;
using TMPro;

namespace Maroon.UI
{
    public class InputField : TMP_InputField, IResetObject
    {
        private string _startText;

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
            text = _startText;
        }
    }
}
