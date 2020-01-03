using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string _key;

        [SerializeField]
        private bool _addColon = false;

        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
            UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (!_text)
                return;

            _text.text = LanguageManager.Instance.GetString(_key);
            if (_addColon)
                _text.text += ":";
        }
    }
}
