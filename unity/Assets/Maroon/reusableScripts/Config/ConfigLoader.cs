using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Maroon.Config
{
    public abstract class ConfigLoader : MonoBehaviour
    {
        protected string _config;
        protected string _configPath;
        protected Dictionary<string, string> _parameters;

        void Start()
        {
            _config = Maroon.GlobalEntities.BootstrappingManager.Instance.UrlParameters[Maroon.WebGlUrlParameter.Config];
            Debug.Log("Config: " + _config);
            
            if(_config == null || _config == "Default") return;

            var experimentName = Maroon.GlobalEntities.SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension;
            _configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Config", experimentName, _config + ".json");
            Debug.Log("ConfigPath: " + _configPath);

            if(_configPath == null) return;

            var configString = System.IO.File.ReadAllText(_configPath);
            _parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(configString);
            Debug.Log("Parameters: " + _parameters);

            LoadConfig();
        }

        public abstract void LoadConfig();
    }
}
