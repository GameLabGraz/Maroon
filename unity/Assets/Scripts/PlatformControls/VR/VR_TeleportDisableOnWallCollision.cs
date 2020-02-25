using UnityEngine;
using VRTK;

namespace PlatformControls.VR
{
    [RequireComponent(typeof(VRTK_CollisionTracker))]
    public class VR_TeleportDisableOnWallCollision : MonoBehaviour
    {
        private VRTK_CollisionTracker _collisionTracker;
        private VRTK_Pointer _pointer;

        private void Start()
        {
            _collisionTracker = GetComponent<VRTK_CollisionTracker>();
            _collisionTracker.TriggerEnter += DisableTeleport;
            _collisionTracker.TriggerExit += EnableTeleport;

            _pointer = GetComponent<VRTK_Pointer>();
        }

        private void OnEnable()
        {
            if(_collisionTracker == null)
                return;

            _collisionTracker.TriggerEnter += DisableTeleport;
            _collisionTracker.TriggerExit += EnableTeleport;
        }

        private void OnDisable()
        {
            if (_collisionTracker == null)
                return;

            _collisionTracker.TriggerEnter -= DisableTeleport;
            _collisionTracker.TriggerExit -= EnableTeleport;
        }

        private void ToggleTeleportsEnabled(bool value)
        {
            if (_pointer)
                _pointer.enabled = value;
        }

        private void DisableTeleport(object sender, CollisionTrackerEventArgs e)
        {
            if (!e.collider.CompareTag("ExcludeTeleport"))
                return;

            ToggleTeleportsEnabled(false);
        }

        private void EnableTeleport(object sender, CollisionTrackerEventArgs e)
        {
            if (!e.collider.CompareTag("ExcludeTeleport"))
                return;

            ToggleTeleportsEnabled(true);
        }
    }
}
