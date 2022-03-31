using GameLabGraz.VRInteraction;
using UnityEngine;

namespace Util
{
    public static class PlayerUtil
    {
        public static bool IsPlayer(GameObject gameObject)
        {
            if (gameObject.CompareTag("Player"))
                return true;

            return gameObject.GetComponent<VRPlayer>() != null;
        }
    }
}
