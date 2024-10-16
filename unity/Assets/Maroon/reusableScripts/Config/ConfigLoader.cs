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
        protected string _parametersString;
        protected Dictionary<string, string> _parameters;

        #if UNITY_WEBGL && !UNITY_EDITOR
        void Start()
        {
            if(Maroon.GlobalEntities.BootstrappingManager.Instance.UrlParameters.TryGetValue(WebGlUrlParameter.Config, out string config))
            {
                var experimentName = Maroon.GlobalEntities.SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension;
                var configPath = Path.Combine(Application.streamingAssetsPath, "Config", experimentName, config + ".json");

                if(configPath == null) return;

                StartCoroutine(LoadJsonWebGL(configPath));
            }
        }
        #endif

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

        protected abstract void SetParameters();
    }
}
