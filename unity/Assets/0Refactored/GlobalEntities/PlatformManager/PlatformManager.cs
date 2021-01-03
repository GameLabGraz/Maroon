using UnityEngine;

namespace Maroon
{
    public class PlatformManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        private static PlatformManager instance = null;
        private Maroon.Platform currentPlatform;
        private bool currentPlatformIsVR = false;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static PlatformManager Instance
        {
            get { return PlatformManager.instance; }
        }

        public Maroon.Platform CurrentPlatform
        {
            get { return this.currentPlatform; }
        }

        public bool CurrentPlatformIsVR
        {
            get { return this.currentPlatformIsVR; }
        }

        public Maroon.SceneType SceneTypeBasedOnPlatform
        {
            get
            {
                if(this.currentPlatformIsVR)
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
            Debug.Log("PlatformManager Awake");

            // Singleton
            if(PlatformManager.instance == null)
            {
                PlatformManager.instance = this;
            }
            else if(PlatformManager.instance != this)
            {
                DestroyImmediate(this.gameObject);
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // Detect platform
            #if UNITY_EDITOR
                this.currentPlatform = Maroon.Platform.Editor;
            #elif UNITY_STANDALONE_WIN
                this.currentPlatform = Maroon.Platform.PC;
            #elif UNITY_WEBGL
                this.currentPlatform = Maroon.Platform.WebGl;
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
            this.currentPlatformIsVR = Maroon.SceneManager.Instance.currentSceneIsVR();
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

