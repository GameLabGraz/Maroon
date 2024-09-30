using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Maroon
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

        // LoadFile method returns a string (file content).
        public void LoadFile(string fileName, System.Action<string> callback)
        {
#if UNITY_WEBGL
            StartCoroutine(LoadFileContentWebGL(fileName, callback));
#else
            string fileContent = LoadFileContentStandalone(fileName);
            callback(fileContent);
#endif
        }

        // Coroutine for WebGL file loading (async)
        private IEnumerator LoadFileContentWebGL(string fileName, System.Action<string> callback)
        {
            string filePath = Application.streamingAssetsPath + "/" + fileName;
            Debug.Log("Loading from WebGL: " + filePath);
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("File loaded successfully from WebGL: " + request.downloadHandler.text);
                // Return file content via callback
                callback(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error loading file in WebGL: " + request.error);
                callback(null); // Return null in case of an error
            }
        }
        
        // Standard builds (PC, Mac, Linux) file loading (sync)
        private string LoadFileContentStandalone(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            Debug.Log("Loading from Standalone: " + filePath);

            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                Debug.Log("File loaded successfully from Standalone: " + fileContents);
                return fileContents;
            }
            else
            {
                Debug.LogError("File not found in Standalone: " + filePath);
                return null;
            }
        }
    }
}
