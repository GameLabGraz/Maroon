using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMP : MonoBehaviour
    {
        [SerializeField]
        private string key;

        [SerializeField]
        private string suffix = string.Empty;

        private TMP_Text _text;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                UpdateLocalizedText();
            }
        }

        private void Start()
        {
            _text = GetComponent<TMP_Text>();

            if (LanguageManager.Instance)
                LanguageManager.Instance.OnLanguageChanged.AddListener(language => UpdateLocalizedText());

            UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (_text == null)
                return;
            if (!LanguageManager.Instance)
                _text.text = Key;
            else
                _text.text = LanguageManager.Instance.GetString(Key) + suffix;
        }
    }
}