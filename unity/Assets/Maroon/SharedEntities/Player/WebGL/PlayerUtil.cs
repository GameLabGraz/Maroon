using UnityEngine;

namespace Util
{
    public static class PlayerUtil
    {
        public static bool IsPlayer(GameObject gameObject)
        {
            if (gameObject.CompareTag("Player"))
                return true;

            return false;
        }
    }
}
