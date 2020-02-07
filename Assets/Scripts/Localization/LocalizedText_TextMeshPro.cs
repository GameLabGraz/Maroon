using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText_TextMeshPro : MonoBehaviour
    {
        [SerializeField]
        public string key;

        [SerializeField]
        private bool addColon = false;

        private TextMeshProUGUI _text;
        private string _oldKey;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            UpdateLocalizedText();
            _oldKey = key;
        }

        private void Update()
        {
            if (_oldKey != key) UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (!_text)
                return;

            if (!LanguageManager.Instance) return;
            _text.text = LanguageManager.Instance.GetString(key);
            if (addColon)
                _text.text += ":";

            _oldKey = key;
        }
    }
}
