using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
#if UNITY_WEBGL
using Maroon.GlobalEntities; // For WebGlReceiver
#endif

namespace Maroon.ReusableScripts.ExperimentParameters
{
    public class ParameterLoader : MonoBehaviour
    {
        [Tooltip("JSON files that can then be loaded via their index and the method LoadJsonFromFileIndex")]
        [SerializeField] private List<TextAsset> _jsonFile = new List<TextAsset>();

        /// <summary>
        /// Invoked when new ExperimentParameters have been loaded.
        /// </summary>
        public UnityEvent<ExperimentParameters> parametersLoaded = new UnityEvent<ExperimentParameters>();

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
            // Listener for external json data (sent e.g. via a Javascript button from a website where Maroon is embedded)
#if UNITY_WEBGL
            WebGlReceiver.Instance.OnIncomingData.AddListener((string jsonData) => { LoadJsonFromString(jsonData); });
#endif
        }

        public List<string> GetJsonNames()
        {
            List<string> names = new List<string>();
            foreach (TextAsset file in _jsonFile)
            {
                names.Add(file.name);
            }

            return names;
        }

        public int IndexOfJson(string name)
        {
            string lowerName = name.ToLower().Replace(" ", "");
            for (int i = 0; i < _jsonFile.Count; i++)
            {
                if (_jsonFile[i].name.ToLower() == lowerName)
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

        public ExperimentParameters LoadJsonFromFileName(string name)
        {
            string lowerName = name.ToLower().Replace(" ", "");
            foreach (TextAsset file in _jsonFile)
            {
                if (file.name.ToLower() == lowerName)
                {
                    return LoadJsonFromString(file.text);
                }
            }

            Debug.LogError("No file with name " + name + " found.");
            return null;
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

            // TODO: ExperimentParamters is expected but ThreeDExperimentParameters is received
            return JsonConvert.DeserializeObject<ExperimentParameters>(data, settings);
        }
        #endregion
    }
}
