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

        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (_text)
                _text.text = LanguageManager.Instance.GetString(key);
        }
    }
}
