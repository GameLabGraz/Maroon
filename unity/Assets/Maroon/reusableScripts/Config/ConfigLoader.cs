using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using SimpleJSON;
using System.IO;
using System.Linq;

namespace Maroon.Config
{
    public class ConfigLoader : MonoBehaviour
    {
        public UnityEvent OnConfigLoaded = new UnityEvent();

        protected string _currentConfigString;
        protected int _currentConfigIndex;
        protected Dictionary<string, string> _configs = new Dictionary<string, string>();

        private static ConfigLoader _instance;

        public string CurrentConfigString => _currentConfigString;

        public int CurrentConfigIndex => _currentConfigIndex;
        
        public static ConfigLoader Instance => _instance;

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

        void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(LoadAllConfigs());
#else
            string basePath = Application.streamingAssetsPath + "/Config/3DMotionSimulation";
            string[] txtFiles = Directory.GetFiles(basePath, "*.json");

            _configs = new Dictionary<string, string>();
            foreach (string file in txtFiles)
            {
                string json = File.ReadAllText(file);
                string cutFilename = file.Replace(basePath + "\\", "");
                cutFilename = cutFilename.Replace(".json", "");
                
                _configs.Add(cutFilename, json);
            }

            ChangeConfig("Default");
#endif
        } 


#if UNITY_WEBGL && !UNITY_EDITOR
        IEnumerator LoadAllConfigs()
        {
            string basePath = "http://localhost:8000/StreamingAssets/Config/3DMotionSimulation/";
            List<string> httpFiles = new List<string>();
            UnityWebRequest uwr = UnityWebRequest.Get("http://localhost:8000/configs.php");
            yield return uwr.SendWebRequest();

            var jsonFile = uwr.downloadHandler.text;
            var parseJSON = JSON.Parse(jsonFile);

            for(int i = 0; i < parseJSON.Count; i++) {
                httpFiles.Add(basePath + parseJSON[i]);
            }

            for(int i = 0; i < httpFiles.Count; i++) {
                UnityWebRequest webReq = UnityWebRequest.Get(httpFiles[i]);
                yield return webReq.SendWebRequest();

                var json = webReq.downloadHandler.text;
                string cutFilename = httpFiles[i].Replace(basePath, "");
                cutFilename = cutFilename.Replace(".json", "");

                _configs.Add(cutFilename, json);
            }
            
            if(Maroon.GlobalEntities.BootstrappingManager.Instance.UrlParameters.TryGetValue(WebGlUrlParameter.Config, out string config))
            {
                ChangeConfig(config);
            }
            else
            {
                ChangeConfig("Default");
            }
        }
#endif

        public void ChangeConfig(string configName)
        {
            foreach (var key in _configs.Keys)
            {
                if (key.ToLower() == configName.Replace(" ", "").ToLower())
                {
                    _currentConfigString = _configs[key];
                    _currentConfigIndex = GetConfigNames().IndexOf(key);
                    OnConfigLoaded.Invoke();
                    return;
                }
            }
        }

        public List<string> GetConfigNames()
        {
            return new List<string>(_configs.Keys.ToList());
        }
    }
}
