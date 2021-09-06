using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class FileController : MonoBehaviour
{
    private static FileController _instance;
    private string _fileType = ".json";
    private string _fileContent = "EMPTY";

    [SerializeField] private List<TextAsset> _jsonFile = new List<TextAsset>();

    private List<Parameters> _parameters;

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

    public List<Parameters> GetParameters()
    {
        return _parameters;
    }

    public List<Parameters> LoadJsonFile(int file)
    {  
        string json_text = _jsonFile[file].text;
        
        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(json_text);
        
        MemoryStream stream = new MemoryStream(byteArray);
        
        using (StreamReader r = new StreamReader(stream))
        {
            
            string json = r.ReadToEnd();
            Debug.Log("Json data: " + json);
            _parameters = JsonConvert.DeserializeObject<List<Parameters>>(json);
        }
        return _parameters;
    }

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

    // Start is called before the first frame update
    void Start()
    {
       
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