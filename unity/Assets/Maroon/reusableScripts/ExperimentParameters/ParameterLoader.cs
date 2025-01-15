using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using Maroon.GlobalEntities;
using System.IO;

// IMPORTS USED FOR WEBGL
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using UnityEngine.Networking;
using System;
#endif

namespace Maroon.ReusableScripts.ExperimentParameters
{
    public class ParameterLoader : MonoBehaviour
    {
        /// <summary>
        /// If true, the JSON files are automatically detected and loaded from the StreamingAssets folder.
        /// </summary>
        [SerializeField] private bool _automaticiallyDetectJsonFiles = false;

        [Tooltip("JSON files that can then be loaded via their index and the method LoadJsonFromFileIndex")]
        [SerializeField] private List<TextAsset> _jsonFile = new List<TextAsset>();

        /// <summary>
        /// Invoked when new ExperimentParameters have been loaded.
        /// </summary>
        public UnityEvent<ExperimentParameters> parametersLoaded = new UnityEvent<ExperimentParameters>();

        /// <summary>
        /// Invoked when the JSON files have been initialized.
        /// </summary>
        public UnityEvent OnFilesInitialized = new UnityEvent();

        /// <summary>
        /// The name of the experiment that is currently loaded.
        /// </summary>
        private string _experimentName;

        /// <summary>
        /// The most recently loaded ExperimentParameters
        /// </summary>
        public ExperimentParameters MostRecentParameters
        {
            get;
            private set;
        }

        #region Singleton
        private static ParameterLoader _instance;
        public static ParameterLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<ParameterLoader>();
                return _instance;
            }
        }
        #endregion


        private void Start()
        {
            _experimentName = SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension;

#if UNITY_WEBGL && !UNITY_EDITOR
            // Listener for external json data (sent e.g. via a Javascript button from a website where Maroon is embedded)
            WebGlReceiver.Instance.OnIncomingData.AddListener((string jsonData) => { LoadJsonFromString(jsonData); });
#endif
            
            if (_automaticiallyDetectJsonFiles)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                StartCoroutine(LoadAllConfigsWebGl());
#else
                LoadAllConfigs();
#endif
            }
        }

        /// <summary>
        /// Returns the names of all JSON files.
        /// </summary>
        /// <returns>List of JSON file names</returns>
        public List<string> GetJsonNames()
        {
            List<string> names = new List<string>();
            foreach (TextAsset file in _jsonFile)
            {
                names.Add(file.name);
            }

            return names;
        }

        /// <summary>
        /// Returns the index of the JSON file with the given name.
        /// </summary>
        /// <param name="name">Name of the JSON file</param>
        /// <returns>Index of the JSON file</returns>
        public int IndexOfJson(string name)
        {
            string modifiedName = name.ToLower().Replace(" ", "");
            for (int i = 0; i < _jsonFile.Count; i++)
            {
                string modifiedFileName = _jsonFile[i].name.ToLower().Replace(" ", "");

                if (modifiedFileName == modifiedName)
                {
                    return i;
                }
            }

            Debug.LogError("No file with name " + name + " found.");
            return -1;
        }

        /// <summary>
        /// Method for when the JSON files are not set in the inspector but are loaded from an external source.
        /// </summary>
        /// <param name="jsonFiles">List of JSON files to load</param>
        public void InitJsonFiles(List<TextAsset> jsonFiles)
        {
            if (_jsonFile.Count > 0)
            {
                Debug.LogWarning("JSON files have already been initialized. Action denied.");
                return;
            }

            _jsonFile = jsonFiles;
            OnFilesInitialized?.Invoke();
        }

        #region Loading of Parameters
        /// <summary>
        /// Method for loading intern JSON-File
        /// </summary>
        /// <param name="file">File to load</param>
        /// <returns>The loaded ExperimentParameters</returns>
        public ExperimentParameters LoadJsonFromFileIndex(int index)
        {
            if (index >= _jsonFile.Count)
            {
                Debug.LogError("Index " + index + " is greater or equal the number of files " + _jsonFile.Count);
                MostRecentParameters = null;
                return null;
            }

            string data = _jsonFile[index].text;
            return LoadJsonFromString(data);
        }

        /// <summary>
        /// Method for loading intern JSON-File via their filename
        /// </summary>
        /// <param name="name">Name of the file to load</param>
        /// <returns>The loaded ExperimentParameters</returns>
        public ExperimentParameters LoadJsonFromFileName(string name)
        {
            int index = IndexOfJson(name);
            if (index == -1)
            {
                Debug.LogError("No file with name " + name + " found.");
                return null;
            }

            return LoadJsonFromFileIndex(index);
        }

        /// <summary>
        /// Method for loading JSON string
        /// </summary>
        /// <param name="data">JSON data</param>
        /// <returns>The loaded ExperimentParameters</returns>
        public ExperimentParameters LoadJsonFromString(string data)
        {
            MostRecentParameters = ConvertJsonToExperimentParameters(data);
            parametersLoaded?.Invoke(MostRecentParameters);
            return MostRecentParameters;
        }

        /// <summary>
        /// Converts a JSON String into ExperimentParameters
        /// </summary>
        /// <param name="data">JSON-format string</param>
        /// <returns>The loaded ExperimentParameters</returns>
        private ExperimentParameters ConvertJsonToExperimentParameters(string data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                // if we allow loading of some sort of external JSON file in the future, then we need to assign a custom SerializationBinder here
            };

            return JsonConvert.DeserializeObject<ExperimentParameters>(data, settings);
        }
        #endregion

        
        /// <summary>
        /// Load all JSON files from the StreamingAssets folder (Non-WebGL)
        /// </summary>
        private void LoadAllConfigs()
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Config", _experimentName);
            string[] txtFiles = Directory.GetFiles(basePath, "*.json");
            List<TextAsset> assets = new List<TextAsset>();

            foreach (string file in txtFiles)
            {
                string jsonText = File.ReadAllText(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                
                TextAsset textAsset = new TextAsset(jsonText);
                textAsset.name = fileName;

                assets.Add(textAsset);
            }

            InitJsonFiles(assets);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        /// <summary>
        /// Load all JSON files from the server (WebGL)
        /// </summary>
        private IEnumerator LoadAllConfigsWebGl()
        {
            string baseDomain = new Uri(Application.absoluteURL).ToString();
            if (baseDomain.Contains("?")) baseDomain = baseDomain.Substring(0, baseDomain.IndexOf('?'));
            string basePath = $"{baseDomain}/StreamingAssets/Config/{_experimentName}/";
            string configListUrl = $"{baseDomain}/configs.php?experimentName={_experimentName}";
            
            List<TextAsset> assets = new List<TextAsset>();
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

                if (webReq.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(webReq.error);
                    continue;
                }

                var jsonText = webReq.downloadHandler.text;
                string fileName = Path.GetFileNameWithoutExtension(httpFiles[i]);
                
                TextAsset textAsset = new TextAsset(jsonText);
                textAsset.name = fileName;

                assets.Add(textAsset);
            }

            InitJsonFiles(assets);
        }
#endif
    }
}
