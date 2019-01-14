//-----------------------------------------------------------------------------
// StartBtnScript.cs
//
// Script for start button
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
/// Script for start button
/// </summary>
[RequireComponent(typeof(Button))]
public class StartBtnScript : MonoBehaviour
{
    /// <summary>
    /// Icon of button when simulation is paused
    /// </summary>
	[SerializeField]
    private Sprite playIcon = null;

    /// <summary>
    /// Icon of button when simulation is running
    /// </summary>
	[SerializeField]
    private Sprite pauseIcon = null;

    /// <summary>
    /// The attached button object
    /// </summary>
    private Button attachedButton;

    private SimulationController simController;

    /// <summary>
    /// Save the button object in member variable
    /// </summary>
    private void Awake()
    {
        attachedButton = gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();
    }

    /// <summary>
    /// Handles the appearance of the button
    /// </summary>
    private void Update()
    {
        if (simController.SimulationRunning &&
            !simController.StepSimulation)
            attachedButton.image.sprite = pauseIcon;
        else
            attachedButton.image.sprite = playIcon;
    }

    /// <summary>
    /// Handles the button being pressed
    /// </summary>
    public void ButtonStartPressed()
    {
        if (simController.SimulationRunning)
            simController.StopSimulation();
        else
            simController.StartSimulation();
    }
}
