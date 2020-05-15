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

/// <summary>
/// Script to show and hide the about panel
/// </summary>
public class AboutInfo : MonoBehaviour
{
    /// <summary>
    /// Indicates if simulation is running
    /// </summary>
    private bool simRunning;

    /// <summary>
    /// Shows the about panel
    /// </summary>
    public void showAboutPanel()
    {
        simRunning = SimulationController.Instance.SimulationRunning;
        if (simRunning)
            SimulationController.Instance.StopSimulation();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the about panel
    /// </summary>
    public void closeAboutIPanel()
    {
        if (simRunning)
            SimulationController.Instance.StartSimulation();
        gameObject.SetActive(false);
    }
}
