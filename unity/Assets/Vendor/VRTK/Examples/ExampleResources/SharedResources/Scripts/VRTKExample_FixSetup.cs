#if UNITY_EDITOR
namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEditor;

    [ExecuteInEditMode]
    public class VRTKExample_FixSetup : MonoBehaviour
    {
        public bool forceOculusFloorLevel = true;
        protected bool trackingLevelFloor = false;

        public virtual void ApplyFixes()
        {
            FixOculus();
        }

        protected virtual void Awake()
        {

            if (Application.isEditor && !Application.isPlaying)
            {
                ApplyFixes();
            }
        }

        protected virtual void Update()
        {
            FixTrackingType();
        }

        protected virtual void FixTrackingType()
        {
#if VRTK_DEFINE_SDK_OCULUS
            if (forceOculusFloorLevel && !trackingLevelFloor)
            {
                // GameObject overManagerGO = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/Oculus/OVRCameraRig");
                // if (overManagerGO != null)
                // {
                //     OVRManager ovrManager = overManagerGO.GetComponent<OVRManager>();
                //     if (ovrManager != null)
                //     {
                //         ovrManager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                //         trackingLevelFloor = true;
                //         Debug.Log("Forced Oculus Tracking to Floor Level");
                //     }
                // }
            }
#endif
        }

        protected virtual void FixOculus()
        {

        }
    }
}
#endif