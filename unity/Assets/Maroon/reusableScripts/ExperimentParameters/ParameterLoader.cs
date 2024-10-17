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
    }
}
