using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string _key;

        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
            UpdateLocalizedText();
        }

        public void UpdateLocalizedText()
        {
            if (_text)
                _text.text = LanguageManager.Instance.GetString(_key);
        }
    }
}
