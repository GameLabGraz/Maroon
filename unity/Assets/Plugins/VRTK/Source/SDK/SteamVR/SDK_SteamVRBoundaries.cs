// SteamVR Boundaries|SDK_SteamVR|005
namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using UnityEngine;
#endif

    /// <summary>
    /// The SteamVR Boundaries SDK script provides a bridge to the SteamVR SDK play area.
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRBoundaries
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseBoundaries
#else
        : SDK_FallbackBoundaries
#endif
    {
        public override void InitBoundaries()
        {
            throw new System.NotImplementedException();
        }

        public override Transform GetPlayArea()
        {
            throw new System.NotImplementedException();
        }

        public override Vector3[] GetPlayAreaVertices()
        {
            throw new System.NotImplementedException();
        }

        public override float GetPlayAreaBorderThickness()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsPlayAreaSizeCalibrated()
        {
            throw new System.NotImplementedException();
        }

        public override bool GetDrawAtRuntime()
        {
            throw new System.NotImplementedException();
        }

        public override void SetDrawAtRuntime(bool value)
        {
            throw new System.NotImplementedException();
        }
    }
}