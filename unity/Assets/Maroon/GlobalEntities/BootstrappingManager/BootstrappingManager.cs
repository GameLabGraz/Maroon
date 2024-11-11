using System.Collections.Generic;
using UnityEngine;

namespace Maroon.GlobalEntities
{
    public class BootstrappingManager : MonoBehaviour, GlobalEntity
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        // Singleton instance
        private static BootstrappingManager _instance = null;

        // Settings
        [SerializeField] private bool _webglEnableSceneLoadingViaUrlParameter = true;
        [SerializeField] private string _webglUrlParameterName = "LoadScene";
        [SerializeField] private CustomSceneAsset _firstStandardScene = null;
        [SerializeField] private CustomSceneAsset _firstVRScene = null;

        // State
        private bool _bootstrappingFinished = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static BootstrappingManager Instance => BootstrappingManager._instance;

        MonoBehaviour GlobalEntity.Instance => Instance;

#if UNITY_WEBGL
        public Dictionary<WebGlUrlParameter, string> UrlParameters { get; private set; }
#endif

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        private void Awake()
        {
            // Singleton
            if(BootstrappingManager._instance == null)
            {
                BootstrappingManager._instance = this;
            }
            else if(BootstrappingManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // Keep alive
            this.transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if(!this._bootstrappingFinished)        
            {
                // Update platform VR state
                PlatformManager.Instance.UpdatePlatformVRStateBasedOnScene();

                // Redirects: Only enable if on bootstrapping scene, if standalone scene, don't redirect somewhere else
                if(SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension == "Bootstrapping")
                {

                    // Stores if SceneManager was already requested to change to another scene
                    bool alreadyRedirected = false;

                    // Webgl redirect
                    // If on WebGL platform and URL redirect enabled
#if UNITY_WEBGL
                    if( (PlatformManager.Instance.CurrentPlatform == Platform.WebGL) &&
                        (this._webglEnableSceneLoadingViaUrlParameter) )
                    {
                        // Read URL parameter
                        this.UrlParameters = Maroon.WebGlUrlParameterReader.GetAllUrlParameters();

                        // Get scene asset
                        if(this.UrlParameters.TryGetValue(WebGlUrlParameter.LoadScene, out string parameter))
                        {
                            Maroon.CustomSceneAsset urlScene;
                            urlScene = SceneManager.Instance.GetSceneAssetBySceneName(parameter + ".pc");

                            // Check if scene requested by parameter exists, and try to load it
                            if((urlScene != null) && (SceneManager.Instance.LoadSceneRequest(urlScene)))
                            {
                                alreadyRedirected = true;
                            }
                        }
                    }
#endif
                    // First Scene Redirect
                    // On any platform, but on WebGL only if not redirected via URL
                    if(!alreadyRedirected)
                    {
                        if(PlatformManager.Instance.CurrentPlatformIsVR)
                        {
                            SceneManager.Instance.LoadSceneRequest(this._firstVRScene);
                        }
                        else
                        {
                            SceneManager.Instance.LoadSceneRequest(this._firstStandardScene);
                        }
                    }
                }

                // Bootstrapping finished
                this._bootstrappingFinished = true;
            }
        }
    }
}

