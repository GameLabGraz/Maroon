//-----------------------------------------------------------------------------
// ResetBtnScript.cs
//
// Script for reset button
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
/// Script for reset button
/// </summary>
[RequireComponent(typeof(Button))]
public class ResetBtnScript : MonoBehaviour
{
    SimulationController simController;

    /// <summary>
    /// Initializing
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        gameObject.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// Handles the appearance of the button
    /// </summary>
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && gameObject.GetComponent<Button>().interactable)
            buttonResetPressed();

        if (!simController.SimulationJustReset)
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Handles the button being pressed and resets the simulation
    /// </summary>
    public void buttonResetPressed()
    {
        simController.ResetSimulation();
    }
}
