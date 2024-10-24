using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Maroon.GlobalEntities
{
    public class StreamingAssetsLoader : MonoBehaviour
    {
        private static StreamingAssetsLoader _instance = null;
        public static StreamingAssetsLoader Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public string ConvertToPascalCase(string input)
        {
            // Convert the input string to title case (capitalize each word)
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titleCase = textInfo.ToTitleCase(input.ToLower());

            // Remove spaces from the title cased string
            return titleCase.Replace(" ", "");
        }

        // LoadFile method returns a string (file content).
        public Task<string> LoadFile(string fileName)
        {
#if UNITY_WEBGL
            return LoadFileContentWebGL(fileName);
#else
            return Task.FromResult(LoadFileContentStandalone(fileName));
#endif
        }

        // Coroutine for WebGL file loading, but returns Task<string> instead of using a callback
        private async Task<string> LoadFileContentWebGL(string fileName)
        {
            string filePath = Application.streamingAssetsPath + "/" + fileName;
            
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error loading file in WebGL: " + request.error);
                return null;
            }
        }

        // Standard builds (PC, Mac, Linux) file loading (sync)
        private string LoadFileContentStandalone(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File not found in Standalone: " + filePath);
                return null;
            }
        }
    }
}
