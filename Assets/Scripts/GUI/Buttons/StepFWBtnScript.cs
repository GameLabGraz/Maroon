//-----------------------------------------------------------------------------
// StepFWBtnScript.cs
//
// Script for forward button
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Script for forward button
/// </summary>
public class StepFWBtnScript : MonoBehaviour
{
    private SimulationController simController;

    private void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
    }

    /// <summary>
    /// Handles the appearance of the button
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && gameObject.GetComponent<Button>().interactable)
            stepFWButtonPressed();

        if (simController.SimulationRunning &&
            !simController.StepSimulation)
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            gameObject.GetComponent<Button>().interactable = true;
        }

    }

    /// <summary>
    /// Handles the button being pressed
    /// </summary>
    public void stepFWButtonPressed()
    {
        simController.SimulateStep();
    }
}
