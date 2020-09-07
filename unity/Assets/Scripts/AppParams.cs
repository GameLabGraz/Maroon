using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Maroon
{

    public class AppParams
    {
        private static AppParams _instance;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        private AppParams()
        {
        }

#if UNITY_WEBGL
        private void ParseParams()
        {
            string url = Application.absoluteURL;
            string queryString = url.Substring(url.IndexOf("?") + 1);
            if (queryString.Length > 0)
            {
                string[] parameters = queryString.Split('&');
                foreach (string parameter in parameters)
                {
                    string[] kvp = parameter.Trim().Split('=');
                    if (kvp[0] != "")
                    {
                        _parameters.Add(kvp[0], UnityWebRequest.UnEscapeURL(kvp[1]));
                    }
                }
            }
        }
#else
    private void ParseParams()
    {
    }
#endif

        public string this[string name]
        {
            get
            {
                if (_parameters.TryGetValue(name, out string value))
                {
                    return value;
                }
                return null;
            }
        }

        public static AppParams Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppParams();
                    _instance.ParseParams();
                }
                return _instance;
            }
        }
    }
}