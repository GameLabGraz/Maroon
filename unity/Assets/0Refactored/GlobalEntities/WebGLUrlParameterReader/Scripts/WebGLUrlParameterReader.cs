using UnityEngine;
using System.Runtime.InteropServices;

namespace Maroon
{
    public class WebGLUrlParameterReader : MonoBehaviour
    {
        // JS implementation at Plugins/WebGLUrlParameterReader.jslib
        [DllImport("__Internal")]
        public static extern string GetUrlParameter(string urlParameterName);
    }
}
