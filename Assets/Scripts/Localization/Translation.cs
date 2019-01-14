using System.Xml.Serialization;

namespace Localization
{
    public class Translation

    {
        [XmlAttribute] public string Key;

        [XmlElement] public string English;

        [XmlElement] public string German;


        public string GetValue(string language)
        {
            switch(language)
            {
                case "German":
                    return German;
                default:
                    return English;
            }
        }
    }
}
