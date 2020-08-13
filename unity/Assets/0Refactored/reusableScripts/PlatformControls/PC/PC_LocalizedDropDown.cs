using System.Collections.Generic;
using GEAR.Localization;
using UnityEngine;
using UnityEngine.UI;


namespace PlatformControls.PC
{
    public class PC_LocalizedDropDown : Dropdown, IResetObject
    {
        private int _startValue;
        private List<OptionData> _keys;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;

            _startValue = value;
            _keys = new List<OptionData>(options);

            UpdateLocalizedOptions();
            if (LanguageManager.Instance)
            {
                LanguageManager.Instance.OnLanguageChanged.AddListener((language) =>
                {
                    UpdateLocalizedOptions();
                });
            }
        }

        private void UpdateLocalizedOptions()
        {
            options.Clear();

            foreach (var key in _keys)
            {
                options.Add(new OptionData(
                    LanguageManager.Instance.GetString(key.text)));
            }

            RefreshShownValue();
        }

        public void ResetObject()
        {
            value = _startValue;
        }
    }
}
