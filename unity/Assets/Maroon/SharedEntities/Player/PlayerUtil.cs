
#if !UNITY_WEBGL
using GameLabGraz.VRInteraction;
#endif

using UnityEngine;

namespace Util
{
    public static class PlayerUtil
    {
        public static bool IsPlayer(GameObject gameObject)
        {
            if (gameObject.CompareTag("Player"))
                return true;

#if UNITY_WEBGL
            return false;
#else
            return gameObject.GetComponent<VRPlayer>() != null;
#endif
        }
    }
}
