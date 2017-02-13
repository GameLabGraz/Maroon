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

    /// <summary>
    /// Shows the about panel
    /// </summary>
    public void showAboutPanel()
    {
        simRunning = SimController.Instance.SimulationRunning;
        if (simRunning)
            SimController.Instance.StopSimulation();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the about panel
    /// </summary>
    public void closeAboutIPanel()
    {
        if (simRunning)
            SimController.Instance.StartSimulation();
        gameObject.SetActive(false);
    }
}
