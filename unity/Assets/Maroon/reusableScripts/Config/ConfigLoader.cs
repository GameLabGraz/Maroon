#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Maroon.Config
{
    public abstract class ConfigLoader : MonoBehaviour
    {
        protected string _config;
        protected string _configPath;
        protected string _parametersString;
        protected Dictionary<string, string> _parameters;

        void Start()
        {
            _config = Maroon.GlobalEntities.BootstrappingManager.Instance.UrlParameters[Maroon.WebGlUrlParameter.Config];
            
            if(_config == null) return;

            var experimentName = Maroon.GlobalEntities.SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension;
            _configPath = Path.Combine(Application.streamingAssetsPath, "Config", experimentName, _config + ".json");

            if(_configPath == null) return;

            StartCoroutine(LoadJsonWebGL(_configPath));
        }

        private IEnumerator LoadJsonWebGL(string url)
        {
            Debug.Log("Loading JSON using UnityWebRequest");
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load JSON file: {request.error}");
                yield break;
            }

            _parametersString = request.downloadHandler.text;
            _parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(_parametersString);

            SetParameters();
        }

        public abstract void SetParameters();
    }
}
#endif
