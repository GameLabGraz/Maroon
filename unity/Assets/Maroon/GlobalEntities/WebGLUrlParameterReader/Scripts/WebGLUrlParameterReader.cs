using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Maroon
{
    public enum WebGlUrlParameter
    {
        LoadScene,
        Config,
    }
    public class WebGlUrlParameterReader : MonoBehaviour
    {
        // JS implementation at Plugins/WebGLUrlParameterReader.jslib
        [DllImport("__Internal")]
        private static extern string _getUrlParameter(string urlParameterName);
        [DllImport("__Internal")]
        private static extern IntPtr _getAllUrlParameters();

        public static string GetUrlParameter(WebGlUrlParameter urlParameter)
        {
            return _getUrlParameter(urlParameter.ToString());
        }

        public static Dictionary<WebGlUrlParameter, string> GetAllUrlParameters()
        {
            string rawJson = Marshal.PtrToStringAnsi(_getAllUrlParameters());
            var rawParameters = JsonUtility.FromJson<Dictionary<string, string>>(rawJson);

            var urlParameters = new Dictionary<WebGlUrlParameter, string>();

            foreach (var key in rawParameters.Keys)
            {
                if (Enum.TryParse(key, out WebGlUrlParameter parameter))
                {
                    urlParameters.Add(parameter, rawParameters[key]);
                }
            }

            foreach (var key in urlParameters.Keys)
            {
                Debug.Log($"WebGLUrlParameterReader: {key} = {urlParameters[key]}");
            }

            return urlParameters;
        }
    }
}
