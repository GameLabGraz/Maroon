
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Collections.Generic;

#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine.Networking;
using System;
using System.Collections;
using Newtonsoft.Json;
#endif

namespace Maroon.Config
{
    public class ConfigLoader : MonoBehaviour
    {
        public UnityEvent OnConfigLoaded = new UnityEvent();

        protected string _currentConfigString;
        protected int _currentConfigIndex;
        protected Dictionary<string, string> _configs = new Dictionary<string, string>();

        public string CurrentConfigString => _currentConfigString;

        public int CurrentConfigIndex => _currentConfigIndex;

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(LoadAllConfigs());
#else
            string basePath = Path.Combine(Application.streamingAssetsPath, "Config", "3DMotionSimulation");
            string[] txtFiles = Directory.GetFiles(basePath, "*.json");

            _configs = new Dictionary<string, string>();
            foreach (string file in txtFiles)
            {
                string json = File.ReadAllText(file);
                string filename = Path.GetFileNameWithoutExtension(file);
                
                _configs.Add(filename, json);
            }

            ChangeConfig("Default");
#endif
        } 

#if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator LoadAllConfigs()
        {
            string baseDomain = new Uri(Application.absoluteURL).ToString();
            if (baseDomain.Contains("?")) baseDomain = baseDomain.Substring(0, baseDomain.IndexOf('?'));
            string basePath = $"{baseDomain}/StreamingAssets/Config/3DMotionSimulation/";
            string configListUrl = $"{baseDomain}/configs.php";
            
            List<string> httpFiles = new List<string>();
            UnityWebRequest uwr = UnityWebRequest.Get(configListUrl);
            
            yield return uwr.SendWebRequest();

            var jsonFile = uwr.downloadHandler.text;
            var parseJSON = JsonConvert.DeserializeObject<List<string>>(jsonFile);

            for(int i = 0; i < parseJSON.Count; i++) {
                httpFiles.Add(basePath + parseJSON[i]);
            }

            for(int i = 0; i < httpFiles.Count; i++) {
                UnityWebRequest webReq = UnityWebRequest.Get(httpFiles[i]);
                yield return webReq.SendWebRequest();

                var json = webReq.downloadHandler.text;
                string filename = Path.GetFileNameWithoutExtension(httpFiles[i]);

                _configs.Add(filename, json);
            }
            
            if(Maroon.GlobalEntities.BootstrappingManager.Instance.UrlParameters.TryGetValue(WebGlUrlParameter.Config, out string config))
            {
                if (!ChangeConfig(config))
                {
                    ChangeConfig("Default");
                }
            }
            else
            {
                ChangeConfig("Default");
            }
        }
#endif

        /// <summary>
        /// Changes current config to the one with the given name.
        /// Returns true if the config was found and changed, false otherwise.
        /// </summary>
        public bool ChangeConfig(string configName)
        {
            string lowerConfigName = configName.ToLower().Replace(" ", "");
            foreach (var key in _configs.Keys)
            {
                if (key.ToLower() == lowerConfigName)
                {
                    _currentConfigString = _configs[key];
                    _currentConfigIndex = GetConfigNames().IndexOf(key);
                    OnConfigLoaded.Invoke();
                    return true;
                }
            }

            return false;
        }

        public List<string> GetConfigNames()
        {
            return new List<string>(_configs.Keys.ToList());
        }
    }
}
