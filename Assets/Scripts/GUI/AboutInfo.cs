//-----------------------------------------------------------------------------
// AboutInfo.cs
//
// Script to show and hide the about panel
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;

/// <summary>
/// Script to show and hide the about panel
/// </summary>
public class AboutInfo : MonoBehaviour
{
    /// <summary>
    /// Indicates if simulation is running
    /// </summary>
    private bool simRunning;

    private SimulationController simController;

    private void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
    }

    /// <summary>
    /// Shows the about panel
    /// </summary>
    public void showAboutPanel()
    {
        simRunning = simController.SimulationRunning;
        if (simRunning)
            simController.StopSimulation();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the about panel
    /// </summary>
    public void closeAboutIPanel()
    {
        if (simRunning)
            simController.StartSimulation();
        gameObject.SetActive(false);
    }
}
