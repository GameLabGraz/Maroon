//-----------------------------------------------------------------------------
// MagnetController.cs
//
// Controller class for the magnet
//
//
// Authors: Michael Stefan Holly
//-----------------------------------------------------------------------------
//

using System.Collections;
using UnityEngine;
using VRTK;

/// <summary>
/// Controller class for the magnet
/// </summary>
public class MagnetController : VRTK_InteractableObject
{
    private bool IsMoving = false;

    private GameObject grabbingObject;

    private SimulationController simController;

    private void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);

        grabbingObject = currentGrabbingObject.gameObject;

        IsMoving = true;

        simController.SimulationRunning = true;

        StartCoroutine(TriggerHapticPulse());
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);

        IsMoving = false;

        simController.SimulationRunning = false;
    }


    private IEnumerator TriggerHapticPulse()
    {
        while (IsMoving)
        {
            var hapticPulseStrength = GetComponent<Magnet>().getExternalForce().magnitude / 10;
            Debug.Log("Haptic Pulse Strength = " + hapticPulseStrength);

            VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(grabbingObject),
                    hapticPulseStrength);

            yield return new WaitForFixedUpdate();
        }
    }

}
