using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class FileController : MonoBehaviour
{
    private string testJSONData = "EMPTY DATA";

    private static FileController _instance;
    private string file_type_ = ".json";
    private string file_content_ = "EMPTY";
    private bool download_done_ = false;
    //private string path_prefix_ = "Assets/Maroon/scenes/experiments/ThrowingBalls/Files/";
    //private string application_path_;
    private string streamassets_path_;
    [SerializeField] private List<TextAsset> json_file_ = new List<TextAsset>();

    //private string path_prefix_ = "/Files/";
    private List<Parameters> parameters_;

    [System.Serializable]
    public class Parameters
    {
        //public string Background { get; set; }
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

    public void testButton()
    {
        loadExternJsonFile(testJSONData);
    }

    public bool getDownloadDone()
    {
        return download_done_;
    }

    public List<Parameters> getParameters()
    {
        return parameters_;
    }

    public List<Parameters> loadJsonFile(int file)
    {
        Debug.Log("LoadJsonFile(" + file + ")");
        
        string json_text = json_file_[file].text;
        
        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(json_text);
        
        MemoryStream stream = new MemoryStream(byteArray);
        
        using (StreamReader r = new StreamReader(stream))
        {
            
            string json = r.ReadToEnd();
            Debug.Log("Json data: " + json);
            parameters_ = JsonConvert.DeserializeObject<List<Parameters>>(json);
        }
        return parameters_;
    }

    public void loadExternJsonFile(string data)
    {
        Debug.Log("Button on website was clicked");
        Debug.Log("loadExternJsonFile() Extern Data: " + data);

        byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(data);

        MemoryStream stream = new MemoryStream(byteArray);

        using (StreamReader r = new StreamReader(stream))
        {
            string json = r.ReadToEnd();
            Debug.Log("Json data: " + json);
            parameters_ = JsonConvert.DeserializeObject<List<Parameters>>(json);
        }

        ParameterUI.Instance.loadExternParametersFromFile(parameters_);
    }

    /*
    private IEnumerator getData(string filename)
    {
        string full_path = System.IO.Path.Combine(streamassets_path_, filename);
        Debug.Log("Full Path getData(): " + full_path);

        if (full_path.Contains("://") || full_path.Contains(":///"))
        {
            WWW www = new WWW(full_path);
            Debug.Log("before yield return: ");
            //yield return new WaitUntil(() => www.bytesDownloaded > 0);
            //yield return www;
            yield return StartCoroutine(this.waitForDownload(www));
            file_content_ = www.text;
            Debug.Log("after yield return: " + www.text);
            
            download_done_ = true;
        }
        else
        {
            file_content_ = System.IO.File.ReadAllText(full_path);
            download_done_ = true;
        }

        
    }

    private IEnumerator waitForDownload(WWW www)
    {
        Debug.Log("waitForDownload ");
        while (!(www.bytesDownloaded > 0))
            yield return null;

        Debug.Log("Download finished");
        yield return new WaitForSeconds(0.1f);
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        //application_path_ = Application.dataPath + "/Resources/JSONFiles/";
        streamassets_path_ = Application.streamingAssetsPath + "/Assessment/ThrowingBalls/";
        testJSONData = json_file_[1].text;
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

    public void dataFromWebsite(string data)
    {
        
    }
}




/*
using (StreamReader r = new StreamReader(streamassets_path_ + filename + file_type_))
{
    string json = r.ReadToEnd();
    parameters_ = JsonConvert.DeserializeObject<List<Parameters>>(json);
    foreach (var par in parameters_)
    {
        //Debug.Log(par.FunctionX);
    }
}
return parameters_;
*/

//Debug.Log("Path: " + application_path_ + filename + file_type_);
//TextAsset txtAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
//string tileFile = txtAsset.text;

//Debug.Log("Content: " + tileFile);

//byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(tileFile);
//MemoryStream stream = new MemoryStream(byteArray);
//Debug.Log("Path: " + tileFile);

// \Assessment\ThrowingBalls


//using (StreamReader r = new StreamReader(stream))
