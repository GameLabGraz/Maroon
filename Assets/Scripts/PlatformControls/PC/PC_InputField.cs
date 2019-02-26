using System.Globalization;
using UnityEngine.UI;

namespace PlatformControls.PC
{
    public class PC_InputField : InputField, IResetObject
    {
        private string _startText;

        protected override void Start()
        {
            _startText = text;
        }

        public void SetText(float value)
        {
            text = value.ToString(CultureInfo.CurrentCulture);
        }

        public void ResetObject()
        {
            text = _startText;
        }
    }
}
