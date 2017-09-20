using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class Translation

{
    [XmlAttribute("Key")]
    public string Key;
    [XmlAttribute("English")]
    public string English;
    [XmlAttribute("German")]
    public string German;
    [XmlAttribute("French")]
    public string French;
    [XmlAttribute("Spain")]
    public string Spain;
    [XmlAttribute("Italian")]
    public string Italian;

    //Get the string for the right language
    public string GetValue(Language language)
    {
        switch(language)
        {
            case Language.English:
                return English;
            case Language.German:
                return German;
            case Language.French:
                return French;
            case Language.Spain:
                return Spain;
            case Language.Italian:
                return Italian;
            default:
                return "";
        }
    }

    public Translation()
    {
        Key = "New Key";
        English = string.Empty;
        German = string.Empty;
        French = string.Empty;
        Spain = string.Empty;
        Italian = string.Empty;
    }

}
