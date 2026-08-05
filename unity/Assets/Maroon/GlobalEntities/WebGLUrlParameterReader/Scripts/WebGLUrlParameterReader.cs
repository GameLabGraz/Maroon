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

        [Obsolete("GetUrlParameter is deprecated due to case-sensitivity. Use 'BootstrappingManager.UrlParameters' instead.")]
        public static string GetUrlParameter(WebGlUrlParameter urlParameter)
        {
            return GetAllUrlParameters().TryGetValue(urlParameter, out string value) ? value : null;
        }

        public static Dictionary<WebGlUrlParameter, string> GetAllUrlParameters()
        {
            // rawJson = {"LoadScene":"Optics","Config":"Default"}
            var rawJson = Marshal.PtrToStringAnsi(_getAllUrlParameters());
            var stringKeyedValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);

            var enumKeyedValues = new Dictionary<WebGlUrlParameter, string>();

            foreach (var kvp in stringKeyedValues)
            {
                // TODO: ignore case sensitivity of enums
                // Enum consists of LoadScene and Config
                if (Enum.TryParse(kvp.Key, true, out WebGlUrlParameter parsedEnum))
                {
                    enumKeyedValues[parsedEnum] = kvp.Value;
                }
                else
                {
                    // Handle the case where the key cannot be parsed into an enum
                    Debug.LogWarning($"Key '{kvp.Key}' does not exist in WebGlUrlParameter enum and will be ignored.");
                }
            }

            return enumKeyedValues;
        }
    }
}
