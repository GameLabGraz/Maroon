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

        IsMoving = true;

        simController.SimulationRunning = true;
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        IsMoving = false;

        simController.SimulationRunning = false;
    }

}
