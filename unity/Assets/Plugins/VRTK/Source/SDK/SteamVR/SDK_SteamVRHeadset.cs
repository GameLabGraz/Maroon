// SteamVR Headset|SDK_SteamVR|003
namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using UnityEngine;
    using System.Collections.Generic;
    using Valve.VR;
#endif

    /// <summary>
    /// The SteamVR Headset SDK script provides a bridge to the SteamVR SDK.
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRHeadset
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseHeadset
#else
        : SDK_FallbackHeadset
#endif
    {
#if VRTK_DEFINE_SDK_STEAMVR
        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public override Transform GetHeadset()
        {
            cachedHeadset = GetSDKManagerHeadset();
            if (cachedHeadset == null)
            {
#if (UNITY_5_4_OR_NEWER)
                SteamVR_Camera foundCamera = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_Camera>(true);
#else
                SteamVR_GameView foundCamera = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_GameView>(true);
#endif
                if (foundCamera != null)
                {
                    cachedHeadset = foundCamera.transform;
                }
            }
            return cachedHeadset;
        }

        /// <summary>
        /// The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public override Transform GetHeadsetCamera()
        {
            cachedHeadsetCamera = GetSDKManagerHeadset();
            if (cachedHeadsetCamera == null)
            {
                SteamVR_Camera foundCamera = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_Camera>(true);
                if (foundCamera != null)
                {
                    cachedHeadsetCamera = foundCamera.transform;
                }
            }
            return cachedHeadsetCamera;
        }

        public override string GetHeadsetType()
        {
            throw new System.NotImplementedException();
        }

        public override Vector3 GetHeadsetVelocity()
        {
            throw new System.NotImplementedException();
        }

        public override Vector3 GetHeadsetAngularVelocity()
        {
            throw new System.NotImplementedException();
        }

        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            throw new System.NotImplementedException();
        }

        public override bool HasHeadsetFade(Transform obj)
        {
            throw new System.NotImplementedException();
        }

        public override void AddHeadsetFade(Transform camera)
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}