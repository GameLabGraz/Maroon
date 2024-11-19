
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Maroon.GlobalEntities;

#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine.Networking;
using System;
using System.Collections;
using Newtonsoft.Json;
#endif

namespace Maroon.Config
{
    public class JsonFileLoader : MonoBehaviour
    {
        public UnityEvent<List<TextAsset>> OnFilesInitialized = new UnityEvent<List<TextAsset>>();
        private string _experimentName;

        private void Start()
        {
            _experimentName = SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension;
            
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(LoadAllConfigs());
#else
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

            OnFilesInitialized.Invoke(assets);
#endif
        } 

#if UNITY_WEBGL && !UNITY_EDITOR
        private IEnumerator LoadAllConfigs()
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

                var jsonText = webReq.downloadHandler.text;
                string fileName = Path.GetFileNameWithoutExtension(httpFiles[i]);
                
                TextAsset textAsset = new TextAsset(jsonText);
                textAsset.name = fileName;

                assets.Add(textAsset);
            }

            OnFilesInitialized.Invoke(assets);
        }
#endif
    }
}
