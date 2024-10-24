using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Maroon;
using System.Threading.Tasks;
using Maroon.GlobalEntities;
using Maroon.Config;

public class ParameterLoader : MonoBehaviour
{
    private static ParameterLoader _instance;
    private Parameters _parameters;
    
    public static ParameterLoader Instance => _instance;

    /// <summary>
    /// Class for storing parameters from a JSON-File
    /// </summary>
    [System.Serializable]
    public class Parameters
    {
        public string Background;
        public string Particle;
        public float T0;
        public float DeltaT;
        public float Steps;
        public float X;
        public float Y;
        public float Z;
        public float Vx;
        public float Vy;
        public float Vz;

        public string m;
        public string fx;
        public string fy;
        public string fz;

        public Dictionary<string, string> expressions = new Dictionary<string, string>();
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ConfigLoader.Instance.OnConfigLoaded.AddListener(OnConfigLoaded);
        }
        else if (_instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Listener for extern json data 
#if UNITY_WEBGL
        WebGlReceiver.Instance.OnIncomingData.AddListener((str) => Debug.Log("Incoming data: " + str));
#endif
    }

    /// <summary>
    /// Getter for the stored parameters
    /// </summary>
    /// <returns>Parameter list</returns>
    public Parameters GetParameters()
    {
        return _parameters;
    }

    public void OnConfigLoaded()
    {
        _parameters = JsonConvert.DeserializeObject<Parameters>(ConfigLoader.Instance.CurrentConfigString);
        ParameterUI.Instance.LoadParameters();
    }
}