using System.Collections;
using PlatformControls.BaseControls;
using UnityEngine;
using VRTK;

namespace PlatformControls.VR
{
    [RequireComponent(typeof(VRTK_Slider))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class VR_MagnetMovement : Movement
    {
        private Magnet _magnet;

        private GameObject _grabbingObject;

        protected void Awake()
        {
            _magnet = GetComponent<Magnet>();

            var minPositionCollider = MinPosition.GetComponent<Collider>();
            var maxPositionCollider = MaxPosition.GetComponent<Collider>();

            var slider = GetComponent<VRTK_Slider>();
            slider.minimumLimit = minPositionCollider ? minPositionCollider : MinPosition.AddComponent<BoxCollider>();
            slider.maximumLimit = maxPositionCollider ? maxPositionCollider : MaxPosition.AddComponent<BoxCollider>();

            var interactableObject = GetComponent<VRTK_InteractableObject>();
            interactableObject.isGrabbable = true;

            interactableObject.InteractableObjectGrabbed += (sender, e) =>
            {
                _grabbingObject = e.interactingObject;
                StartMoving();
                StartCoroutine(TriggerHapticPulse());
            };

            interactableObject.InteractableObjectUngrabbed += (sender, e) => StopMoving();
        }

        private IEnumerator TriggerHapticPulse()
        {
            while (IsMoving)
            {
                var hapticPulseStrength = _magnet.getExternalForce().magnitude / 10;

                VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(_grabbingObject),
                    hapticPulseStrength);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
