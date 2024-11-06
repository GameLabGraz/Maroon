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

        /// <summary>
        /// Used to know whether there was already an experiment which potentially used the URL fragment parameters on WebGL already
        /// </summary>
        private bool firstExperimentDefaultParametersLoaded = false;

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
        /// <param name="firstDefaultParametersLoad">If a new experiment has just loaded and this is call should load the initial default parameters.
        /// If true, might be ignored and instead the URL fragment parameters will be used.</param>
        /// <returns>The loaded ExperimentParameters</returns>
        public ExperimentParameters LoadJsonFromFileIndex(int index, bool firstDefaultParametersLoad = false)
        {
#if UNITY_WEBGL
            /*
             * Initial config received from Javascript (originating from the URL Fragment config) will be received before the requested experiment is loaded,
             * thus if on WebGL and the WebGlReceiver.Instance.MostRecentData is not null and this it the first experiment, load instead the WebGlReceiver.Instance.MostRecentData instead of the requested default file.
             */
            if (firstDefaultParametersLoad && !firstExperimentDefaultParametersLoaded && !string.IsNullOrWhiteSpace(WebGlReceiver.Instance.MostRecentData))
            {
                firstExperimentDefaultParametersLoaded = true;
                return LoadJsonFromString(WebGlReceiver.Instance.MostRecentData);
            }
#endif

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
            Debug.Log("Trying to load ExperimentParameters from JSON String.");
            MostRecentParameters = ConvertJsonToExperimentParameters(data);
            if (MostRecentParameters == null)
            {
                Debug.LogError("Loaded ExperimentParameters are null.");
            }
            else
            {
                Debug.Log("Successfully parsed ExperimentParameters: " + MostRecentParameters.GetType());
            }
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
