using TMPro;
using UnityEngine;
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

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (!_text)
                return;

            _text.text = LanguageManager.Instance.GetString(key);
            if (addColon)
                _text.text += ":";
        }
    }
}
