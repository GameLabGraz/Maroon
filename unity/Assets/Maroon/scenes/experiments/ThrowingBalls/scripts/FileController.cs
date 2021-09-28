using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class FileController : MonoBehaviour
{
    private static FileController _instance;
    [SerializeField] private List<TextAsset> _jsonFile = new List<TextAsset>();
    private List<Parameters> _parameters;

    /// <summary>
    /// Class for storing parameters from a JSON-File
    /// </summary>
    [System.Serializable]
    public class Parameters
    {
        public string Background;
        public string Particle;
        public string FunctionX;
        public string FunctionY;
        public string FunctionZ;
        public float Mass;
        public float T0;
        public float DeltaT;
        public float Steps;
        public float X;
        public float Y;
        public float Z;
        public float Vx;
        public float Vy;
        public float Vz;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Getter for the stored parameters
    /// </summary>
    /// <returns>Parameter list</returns>
    public List<Parameters> GetParameters()
    {
        return _parameters;
    }

    /// <summary>
    /// Method for loading intern JSON-File
    /// </summary>
    /// <param name="file">File to load</param>
    /// <returns>Parameters</returns>
    public List<Parameters> LoadJsonFile(int file)
    {  
        string json_text = _jsonFile[file].text;
        
        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(json_text);
        
        MemoryStream stream = new MemoryStream(byteArray);
        
        using (StreamReader r = new StreamReader(stream))
        {
            
            string json = r.ReadToEnd();
            //Debug.Log("Json data: " + json);
            _parameters = JsonConvert.DeserializeObject<List<Parameters>>(json);
        }
        return _parameters;
    }

    /// <summary>
    /// Method for loading extern JSON-File
    /// Triggers the calculation process
    /// </summary>
    /// <param name="data">JSON data</param>
    public void LoadExternJsonFile(string data)
    {
        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(data);

        MemoryStream stream = new MemoryStream(byteArray);

        using (StreamReader r = new StreamReader(stream))
        {
            string json = r.ReadToEnd();
            _parameters = JsonConvert.DeserializeObject<List<Parameters>>(json);
        }

        ParameterUI.Instance.LoadExternParametersFromFile(_parameters);
    }

    public static FileController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<FileController>();
            return _instance;
        }
    }
}