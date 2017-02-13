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

    /// <summary>
    /// Handles the appearance of the button
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && gameObject.GetComponent<Button>().interactable)
            stepFWButtonPressed();

        if (SimController.Instance.SimulationRunning &&
            !SimController.Instance.StepSimulation)
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
        SimController.Instance.SimulateStep();
    }
}
