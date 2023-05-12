using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Maroon.GlobalEntities;

namespace Maroon.PlatformControls.VR
{
    public class TeleportPointAdder : MonoBehaviour
    {
        [SerializeField] private string teleportPointTitle;

        private IEnumerator Start()
        {
            if (PlatformManager.Instance == null)
            {
                yield return new WaitUntil(() => PlatformManager.Instance != null);
            }

            if (PlatformManager.Instance.CurrentPlatformIsVR)
            {
                TeleportPoint teleportPoint = gameObject.AddComponent<TeleportPoint>();
                teleportPoint.title = teleportPointTitle;
            }
        }
    }
}
