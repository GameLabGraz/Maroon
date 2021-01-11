using UnityEngine;

namespace Maroon
{
    public class PlatformManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        private static PlatformManager _instance = null;

        private Maroon.Platform _currentPlatform;
        
        private bool _currentPlatformIsVR = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static PlatformManager Instance
        {
            get { return PlatformManager._instance; }
        }

        public Maroon.Platform CurrentPlatform
        {
            get { return this._currentPlatform; }
        }

        public bool CurrentPlatformIsVR
        {
            get { return this._currentPlatformIsVR; }
        }

        public Maroon.SceneType SceneTypeBasedOnPlatform
        {
            get
            {
                if(this._currentPlatformIsVR)
                {
                    return Maroon.SceneType.VR;
                }

                return Maroon.SceneType.Standard;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        private void Awake()
        {
            // Singleton
            if(PlatformManager._instance == null)
            {
                PlatformManager._instance = this;
            }
            else if(PlatformManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // Detect platform
            #if UNITY_EDITOR
                this._currentPlatform = Maroon.Platform.Editor;
            #elif UNITY_STANDALONE_WIN
                this.currentPlatform = Maroon.Platform.PC;
            #elif UNITY_WEBGL
                this.currentPlatform = Maroon.Platform.WebGL;
            #elif UNITY_STANDALONE_OSX
                this.currentPlatform = Maroon.Platform.Mac;
            #elif UNITY_ANDROID
                this.currentPlatform = Maroon.Platform.Android;
            #elif UNITY_IOS 
                this.currentPlatform = Maroon.Platform.iOS;
            #endif
        }

        public void UpdatePlatformVRStateBasedOnScene()
        {
            this._currentPlatformIsVR = Maroon.SceneManager.Instance.activeSceneIsVR();
        }
    }

    public enum Platform
    {
        Editor,
        PC,
        WebGL,
        Mac,
        Android,
        iOS
    }
}

