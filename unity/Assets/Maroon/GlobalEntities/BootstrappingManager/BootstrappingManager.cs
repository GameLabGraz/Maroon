using UnityEngine;

namespace Maroon
{
    public class BootstrappingManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        // Singleton instance
        private static BootstrappingManager _instance = null;

        // Settings
        [SerializeField] private bool _webglEnableSceneLoadingViaUrlParameter = true;
        [SerializeField] private string _webglUrlParameterName = "LoadScene";
        [SerializeField] private Maroon.CustomSceneAsset _firstStandardScene = null;
        [SerializeField] private Maroon.CustomSceneAsset _firstVRScene = null;

        // State
        private bool _bootstrappingFinished = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static BootstrappingManager Instance
        {
            get { return BootstrappingManager._instance; }
        }

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
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if(!this._bootstrappingFinished)        
            {
                // Update platform VR state
                Maroon.PlatformManager.Instance.UpdatePlatformVRStateBasedOnScene();

                // Redirects: Only enable if on bootstrapping scene, if standalone scene, don't redirect somewhere else
                if(Maroon.SceneManager.Instance.ActiveSceneNameWithoutPlatformExtension == "Bootstrapping")
                {

                    // Stores if SceneManager was already requested to change to another scene
                    bool alreadyRedirected = false;

                    // Webgl redirect
                    // If on WebGL platform and URL redirect enabled
                    if( (Maroon.PlatformManager.Instance.CurrentPlatform == Maroon.Platform.WebGL) &&
                        (this._webglEnableSceneLoadingViaUrlParameter) )
                    {
                        // Read URL parameter
                        string parameter = Maroon.WebGLUrlParameterReader.GetUrlParameter(this._webglUrlParameterName);

                        // Get scene asset
                        Maroon.CustomSceneAsset urlScene;
                        urlScene = Maroon.SceneManager.Instance.GetSceneAssetBySceneName(parameter + ".pc");

                        // Check if scene requested by parameter exists, and try to load it
                        if((urlScene != null) && (Maroon.SceneManager.Instance.LoadSceneRequest(urlScene)))
                        {
                            alreadyRedirected = true;
                        }
                    }

                    // First Scene Redirect
                    // On any platform, but on WebGL only if not redirected via URL
                    if(!alreadyRedirected)
                    {
                        if(Maroon.PlatformManager.Instance.CurrentPlatformIsVR)
                        {
                            Maroon.SceneManager.Instance.LoadSceneRequest(this._firstVRScene);
                        }
                        else
                        {
                            Maroon.SceneManager.Instance.LoadSceneRequest(this._firstStandardScene);
                        }
                    }
                }

                // Bootstrapping finished
                this._bootstrappingFinished = true;
            }
        }
    }
}

