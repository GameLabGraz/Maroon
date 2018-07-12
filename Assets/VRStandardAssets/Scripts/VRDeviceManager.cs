using UnityEngine;
using UnityEngine.VR;

namespace VRStandardAssets.Utils
{
    // This class exists to setup the device on a per platform basis.
    // The class uses the singleton pattern so that only one object exists.
    public class VRDeviceManager : MonoBehaviour
    {
#pragma warning disable 0414
        [SerializeField] private float m_RenderScale = 1.4f;
#pragma warning restore 0414

        private static VRDeviceManager s_Instance;


        public static VRDeviceManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<VRDeviceManager> ();
                    DontDestroyOnLoad (s_Instance.gameObject);
                }

                return s_Instance;
            }
        }


        private void Awake ()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad (this);
            }
            else if (this != s_Instance)
            {
                Destroy (gameObject);
            }

            SetupVR ();
        }


        private void SetupVR ()
        {
            //Gear VR does not currently support renderScale
#if !UNITY_ANDROID
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = m_RenderScale;
#endif

#if UNITY_STANDALONE
            UnityEngine.XR.XRSettings.loadedDevice = VRDeviceType.Oculus;
#endif
            
#if UNITY_PS4 && !UNITY_EDITOR
		    VRSettings.loadedDevice = VRDeviceType.Morpheus;
#endif

            UnityEngine.XR.XRSettings.enabled = true;
        }
    }
}