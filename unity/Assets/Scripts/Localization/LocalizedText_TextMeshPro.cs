using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText_TextMeshPro : MonoBehaviour
    {
        [SerializeField]
        public string key;

        [SerializeField]
        private bool addColon = false;

        private TMP_Text _text;
        private string _oldKey;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
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
            var text = LanguageManager.Instance.GetString(key);
            // _text.text = LanguageManager.Instance.GetString(key);
            if (addColon)
                text += ":";

            _text.text = text;
            _oldKey = key;
        }
    }
}
