using UnityEngine;

namespace Maroon.GlobalEntities
{
    public class PlatformManager : MonoBehaviour, GlobalEntity
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        private static PlatformManager _instance = null;

        private Platform _currentPlatform;
        
        private bool _currentPlatformIsVR = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static PlatformManager Instance => PlatformManager._instance;

        MonoBehaviour GlobalEntity.Instance => Instance;

        public Platform CurrentPlatform => this._currentPlatform;

        public bool CurrentPlatformIsVR => this._currentPlatformIsVR;

        public SceneType SceneTypeBasedOnPlatform => this._currentPlatformIsVR ? SceneType.VR : SceneType.Standard;

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
                this._currentPlatform = Platform.Editor;
            #elif UNITY_STANDALONE_WIN
                this._currentPlatform = Platform.PC;
            #elif UNITY_WEBGL
                this._currentPlatform = Platform.WebGL;
            #elif UNITY_STANDALONE_OSX
                this._currentPlatform = Platform.Mac;
            #elif UNITY_ANDROID
                this._currentPlatform = Platform.Android;
            #elif UNITY_IOS 
                this._currentPlatform = Platform.iOS;
            #endif
        }

        public void UpdatePlatformVRStateBasedOnScene()
        {
            this._currentPlatformIsVR = SceneManager.Instance.activeSceneIsVR();
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

