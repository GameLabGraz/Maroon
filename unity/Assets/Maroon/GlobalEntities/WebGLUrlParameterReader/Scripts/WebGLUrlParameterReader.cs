using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

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
            var param = _getUrlParameter(urlParameter.ToString());
            Debug.Log($"WebGLUrlParameterReader: {urlParameter} = {param}");
            return param;
        }

        public static Dictionary<WebGlUrlParameter, string> GetAllUrlParameters()
        {
            // rawJson = {"LoadScene":"Optics","Config":"Default"}
            var rawJson = Marshal.PtrToStringAnsi(_getAllUrlParameters());
            var values = JsonConvert.DeserializeObject<Dictionary<WebGlUrlParameter, string>>(rawJson);
            return values;
        }
    }
}
