using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Localization
{
    [XmlRoot("LanguageManager")]
    public class LanguageManager
    {
        [HideInInspector]
        public static LanguageManager Instance = null; //so we have access to Manager from other files

        [XmlIgnore]
        private string _currentLanguage = "English";

        public string CurrentLanguage
        {
            get { return _currentLanguage;}
            set { _currentLanguage = value; }
        }

        //list of every dialogue and his translations
        [XmlArray("Translations"), XmlArrayItem("Translation")]
        public List<Translation> Translations = new List<Translation>();

        public LanguageManager()
        {
            Instance = this;
        }

        //returns True if key is existing. Very nice for small games
        public bool KeyExists(string key)
        {
            return GetIndex(key) > -1;
        }

        //returns the dialogue in the current language for given key if key exists
        public string GetString(string key)
        {
            return GetString(key, _currentLanguage);
        }
    
        //returns the dialogue in the chosen language for given key if key exists
        public string GetString(string key, string language)
        {
            var index = GetIndex(key);
            //If Index exists
            return index > -1 ? Translations[index].GetValue(language) : key;
        }

        //add new dialogue in Translation list or update existing dialogue
        private void Insert(Translation translation, bool add)
        {
            var i = GetIndex(translation.Key);
            if (i > -1)
            {
                //do nothing if key already exists
                if (add)
                    return;
                //updating
                Translations[i] = translation;
            }

            //add
            if (add)
                Translations.Add(translation);
        }

        public void Add(Translation translation)
        {
            Insert(translation, true);
        }

        public void Update(Translation translation)
        {
            Insert(translation, false);
        }

        //Remove dialogue at given key
        public void Remove(string key)
        {
            var i = GetIndex(key);
            if (i > -1)
                Translations.RemoveAt(i);
        }

        //Remove dialogue at given index
        public void RemoveIndex(int index)
        {
            Translations.RemoveAt(index);
        }


        //go through whole list, quite ok for small games
        private int GetIndex(string key)
        {
            for (var i = 0; i < Translations.Count; i++)
            {
         
                if (Translations[i].Key.Equals(key))
                    return i;
            }
            return -1;
        }

        //XML Stuff Saving
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(LanguageManager));
       
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
                Debug.Log("Saving");    
            }  
        }
    
        //XML Stuff Loading
        public static LanguageManager Load(string path)
        {
            var xmlSerializer = new XmlSerializer(typeof(LanguageManager));
            try
            {
                var text = Resources.Load<TextAsset>(path);
                if (text == null)
                    return null;

                var stream = new MemoryStream(text.bytes);       
                return xmlSerializer.Deserialize(stream) as LanguageManager;

            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }
    }
}