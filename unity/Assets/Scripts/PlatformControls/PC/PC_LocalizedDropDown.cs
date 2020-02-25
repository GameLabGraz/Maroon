using Localization;
using UnityEngine;
using UnityEngine.UI;


namespace PlatformControls.PC
{
    public class PC_LocalizedDropDown : Dropdown, IResetObject
    {
        private int _startValue;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;

            _startValue = value;

            // Translate option keys
            foreach (var option in options)
                option.text = LanguageManager.Instance.GetString(option.text);

            RefreshShownValue();
        }

        public void ResetObject()
        {
            value = _startValue;
        }
    }
}
