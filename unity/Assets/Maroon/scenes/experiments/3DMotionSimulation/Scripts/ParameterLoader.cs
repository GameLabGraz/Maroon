using UnityEngine;
using Newtonsoft.Json;
using Maroon.Config;
using Maroon.Physics.ThreeDimensionalMotion;

namespace Maroon.Parameter
{
    public class ParameterLoader : MonoBehaviour
    {
        private static ParameterLoader _instance;
        private ThreeDimensionalMotionParameters _parameters;
        
        public static ParameterLoader Instance => _instance;

        [SerializeField] private ConfigLoader _configLoader;

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

            transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Getter for the stored parameters
        /// </summary>
        /// <returns>Parameter list</returns>
        public ThreeDimensionalMotionParameters GetParameters()
        {
            return _parameters;
        }

        public void OnConfigLoaded()
        {
            _parameters = JsonConvert.DeserializeObject<ThreeDimensionalMotionParameters>(_configLoader.CurrentConfigString);
            ParameterUI.Instance.LoadParameters();
        }
    }
}