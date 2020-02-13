using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

namespace Localization
{
    public class LanguageManager : MonoBehaviour
    {
        [SerializeField]
        private string _mlgFile;

        public static LanguageManager Instance { get; private set; }

        public SystemLanguage CurrentLanguage { get; set; } = SystemLanguage.English;

        private readonly Dictionary<string, Translation> _translations = new Dictionary<string, Translation>();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (Instance == null)
            {
                Instance = this;
                Instance.CurrentLanguage = Application.systemLanguage;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            Instance.LoadMlgFile(_mlgFile);
        }

        public string GetString(string key)
        {
            return GetString(key, CurrentLanguage);
        }
        
        public string GetString(string key, SystemLanguage language)
        {
            return _translations.ContainsKey(key) ?_translations[key].GetValue(language) : key;
        }

        public void LoadMlgFile(string path)
        {
            Instance._translations.Clear();

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
                if(key == null)
                    continue;

                var translation = new Translation(key);
                foreach (var languageElement in translationElement.Elements())
                {
                    if(!Enum.TryParse(languageElement.Name.ToString(), out SystemLanguage language))
                        continue;

                    translation.AddTranslation(language, Regex.Unescape(languageElement.Value));
                }

                _translations.Add(key, translation);
            }
        }
    }
}