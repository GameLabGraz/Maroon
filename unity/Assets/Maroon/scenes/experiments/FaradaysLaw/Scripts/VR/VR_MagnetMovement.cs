using System.Collections;
using PlatformControls.BaseControls;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace PlatformControls.VR
{
    [RequireComponent(typeof(Magnet))]
    [RequireComponent(typeof(Interactable))]
    public class VR_MagnetMovement : Movement
    {
        [Range(0f, 320f)] public float hapticFeedbackFrequency = 100f;
        
        private Magnet _magnet;
        private Hand _hand;

        protected void Awake()
        {
            _magnet = GetComponent<Magnet>();
        }
        
        private void OnAttachedToHand( Hand hand )
        {
            Debug.Log("Magnet attached to hand");
            _hand = hand;
            StartMoving();
            StartCoroutine(TriggerHapticPulse());
        }

        private void OnDetachedFromHand( Hand hand )
        {
            Debug.Log("Magnet detached From hand");

            _hand = null;
            StopMoving();
        }
        
        private IEnumerator TriggerHapticPulse()
        {
            while (IsMoving)
            {
                var hapticPulseStrength = _magnet.GetExternalForce().magnitude / 10;
                
                _hand.TriggerHapticPulse(Time.fixedDeltaTime, hapticFeedbackFrequency, hapticPulseStrength);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
