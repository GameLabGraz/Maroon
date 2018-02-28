using UnityEngine;
using VRTK;

public class MagnetHaptics : VRTK_InteractHaptics
{
    private Magnet magnet;

    private void Start()
    {
        if(!(magnet = GetComponent<Magnet>()))
            Debug.LogError("The `MagnetHaptics` script is required to be attached to a GameObject that has the `Magnet` script also attached to it.");
    }

    public override void HapticsOnGrab(VRTK_ControllerActions controllerActions)
    {
        float strength = magnet.getExternalForce().magnitude;
        TriggerHapticPulse(controllerActions, strength * strengthOnGrab, durationOnGrab, intervalOnGrab);
    }
}
