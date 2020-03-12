using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Localization
{
    [Serializable] public class LanguageChangedEvent : UnityEvent<SystemLanguage> { }

    public class LanguageManager : MonoBehaviour
    {
        [SerializeProperty("CurrentLanguage")]
        public SystemLanguage _currentLanguage = SystemLanguage.English;

        public SystemLanguage CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                OnLanguageChanged.Invoke(_currentLanguage);
            }
        }

        [SerializeField]
        private List<string> _mlgFiles;

        public static LanguageManager Instance { get; private set; }


        public LanguageChangedEvent OnLanguageChanged = new LanguageChangedEvent();

        private readonly Dictionary<string, Translation> _translations = new Dictionary<string, Translation>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (Instance == null)
            {
                Instance = this;
                Instance.CurrentLanguage = SystemLanguage.German;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            ClearTranslations();

            foreach (var mlgFile in _mlgFiles)
            {
                Instance.LoadMlgFile(mlgFile);
            }

        }

        public void ClearTranslations()
        {
            Instance._translations.Clear();
        }

        public string GetString(string key)
        {
            return GetString(key, CurrentLanguage);
        }

        public string GetString(string key, SystemLanguage language)
        {
            return _translations.ContainsKey(key) ? _translations[key].GetValue(language) : key;
        }

        public void LoadMlgFile(string path)
        {
            var text = Resources.Load<TextAsset>(path);
            if (!text)
            {
                Debug.LogError($"LanguageManager::LoadMlgFile: Unable to load language file {path}");
                return;
            }

            var doc = XDocument.Load(new MemoryStream(text.bytes));
            foreach (var translationElement in doc.Descendants("Translation"))
            {
                var key = translationElement.Attribute("Key")?.Value;
                if (key == null)
                    continue;

                var translation = new Translation(key);
                foreach (var languageElement in translationElement.Elements())
                {
                    if (!Enum.TryParse(languageElement.Name.ToString(), out SystemLanguage language))
                        continue;

                    translation.AddTranslation(language, languageElement.Value);
                }

                _translations[key] = translation;
            }
        }
    }
}