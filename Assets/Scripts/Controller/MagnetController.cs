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

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        base.Grabbed(currentGrabbingObject);

        grabbingObject = currentGrabbingObject;

        IsMoving = true;

        simController.SimulationRunning = true;

        StartCoroutine(TriggerHapticPulse());
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        IsMoving = false;

        simController.SimulationRunning = false;
    }


    private IEnumerator TriggerHapticPulse()
    {
        while (IsMoving)
        {
            grabbingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)(GetComponent<Magnet>().getExternalForce().magnitude));

            yield return new WaitForFixedUpdate();
        }
    }

}
