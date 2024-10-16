﻿using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Maroon.GlobalEntities;

public class ParameterLoader : MonoBehaviour
{
    private static ParameterLoader _instance;
    [SerializeField] private List<TextAsset> _jsonFile = new List<TextAsset>();
    private Parameters _parameters;

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

    // Start is called before the first frame update
    private void Start()
    {
        // Listener for extern json data 
#if UNITY_WEBGL
        WebGlReceiver.Instance.OnIncomingData.AddListener(HandleExternalJson);
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

    /// <summary>
    /// Method for loading intern JSON-File
    /// </summary>
    /// <param name="file">File to load</param>
    /// <returns>Parameters</returns>
    public Parameters LoadJsonFromFile(int index)
    {
        string data = _jsonFile[index].text;

        return LoadJson(data);
    }

    /// <summary>
    /// Method for loading extern JSON-File
    /// Triggers the calculation process
    /// 
    /// Example javascript code and json format
    /// var data = JSON.stringify([{ 
    /// Background: "Grass", 
    /// Particle: "Ball", 
    /// FunctionX: "(-0.01*(vx-(1))-0.03*(vx-(1))*sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))", 
    /// FunctionY: "(-0.01*(vy-(7*Exp(-x*x)))-0.03*(vy-(7*Exp(-x*x)))*Sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))", 
    /// FunctionZ: "(-0.01*(vz-(-3*Exp(-t*t)))-0.03*(vz-(-3*Exp(-t*t)))*Sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))-9.81*0.1", 
    /// Mass: "0.1", 
    /// T0: "0", 
    /// DeltaT: "0.01", 
    /// Steps: "100", 
    /// X: "0", Y: "0", Z: "0", 
    /// Vx: "-7", Vy: "5", Vz: "10"
    /// }]);
    //
    /// gameInstance.SendMessage('WebGL Receiver', 'GetDataFromJavaScript', data);
    /// </summary>
    /// <param name="data">JSON data</param>
    public void HandleExternalJson(string data)
    {
        var parameters = LoadJson(data);
        ParameterUI.Instance.LoadParameters(parameters);
    }

    public Parameters LoadJson(string data)
    {
        _parameters = JsonConvert.DeserializeObject<Parameters>(data);

        return _parameters;
    }

    public static ParameterLoader Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ParameterLoader>();
            return _instance;
        }
    }
}