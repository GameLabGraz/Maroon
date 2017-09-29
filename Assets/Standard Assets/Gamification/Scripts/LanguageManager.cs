using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif




//enum of all languages the game supports. Can be expanded as desired. For now just English, German and French are supported
//Remember to make changes also in Translation.cs script
public enum Language
{
    English,
    German,
    French,
    Spain,
    Italian
}

[XmlRoot("LanguageManager")]
public class LanguageManager
{
   
    public static LanguageManager instance = null; //so we have access to Manager from other files
    [HideInInspector]
    //Path of XML
    private static string path = "Languages";
    private static string savePath = Path.Combine(Application.dataPath, "Resources/Languages.xml");
    //Current language
    [XmlEnum("CurrentLanguage")]
    public Language currentLanguage;
    //list of every dialogue and his translations
    [XmlArray("Translations"), XmlArrayItem("Translation")]
    public List<Translation> Translations;



    public LanguageManager()
    {
        Translations = new List<Translation>();
        instance = this;
    }

    //Change currentLanguage
    public void SetCurrentLanguage(Language l)
    {
        currentLanguage = l;
    }

    public string getPath()
    {
        return path;
    }

    //returns True if key is existing. Very nice for small games
    public bool KeyExisting(string key)
    {
        return FindIndex(key) > -1;
    }


    //returns the dialogue in the current language for given key if key exists
    public string GetString(string key)
    {
        int index = FindIndex(key);
        //If Index exists
        if (index > -1)
            return Translations[index].GetValue(currentLanguage);
        return "";
    }
    
    //returns the dialogue in the chosen language for given key if key exists
    public string GetString(string key, Language language)
    {
        int index = FindIndex(key);
        //If Index exists
        if (index > -1)
            return Translations[index].GetValue(language);
        return "";
    }

    //add new dialogue in Translation list or update existing dialogue
    private void Insert(Translation translation, bool add)
    {
        int i = FindIndex(translation.Key);
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
        int i = FindIndex(key);
        if (i > -1)
            Translations.RemoveAt(i);
    }

    //Removde dialogue at given index
    public void RemoveIndex(int index)
    {
        Translations.RemoveAt(index);
    }



    //go through whole list, quite ok for small games
    private int FindIndex(string key)
    {
        for (int i = 0; i < Translations.Count; i++)
        {
         
            if (Translations[i].Key.Equals(key))
                return i;
        }
        return -1;
    }

    //XML Stuff Saving
    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LanguageManager));
       
        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            serializer.Serialize(stream, this);
            stream.Close();
        }
        //Avoid Backup Bug
        AssetDatabase.Refresh();
    }
    
    //XML Stuff Loading
    public static LanguageManager Load()
    {
            Stream stream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LanguageManager));

        try
        {
                TextAsset text = Resources.Load("Languages") as TextAsset;
                stream = new MemoryStream(text.bytes);
                        
                return xmlSerializer.Deserialize(stream) as LanguageManager;                    
        }
        catch
        {
            return new LanguageManager();
        }
        

             
          
     
        /*
        if (File.Exists(path))
        {

            XmlSerializer serializer = new XmlSerializer(typeof(LanguageManager));



        

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as LanguageManager;
            }
        }
        else
        {
            Debug.Log("ERROR: XML Path Not Existing");
            return new LanguageManager();
        }*/

    }

   
}

