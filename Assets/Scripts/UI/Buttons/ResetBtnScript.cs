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
using UnityEngine.UI;

/// <summary>
/// Script for reset button
/// </summary>
[RequireComponent(typeof(Button))]
public class ResetBtnScript : MonoBehaviour
{
    public bool AllowWholeReset = false;
    public bool AllowWithButtonPress = true;
    
    SimulationController simController;

    private bool _inWholeResetMode = false;
    private CanvasRenderer _canvasRenderer;
    private Button _button;

    /// <summary>
    /// Initializing
    /// </summary>
    void Start()
    {
        _button = gameObject.GetComponent<Button>();
        _canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        DeactivateResetButton();
    }

    /// <summary>
    /// Handles the appearance of the button
    /// </summary>
	void Update()
    {
        if (AllowWithButtonPress && Input.GetKeyDown(KeyCode.LeftArrow) && _button.interactable)
        {
            Debug.Log("Reset san");
            buttonResetPressed();
        }

        if (!simController.SimulationJustReset)
        {
            ActivateResetButton();
        }
        else
        {
            DeactivateResetButton();
        }
    }

    /// <summary>
    /// Handles the button being pressed and resets the simulation
    /// </summary>
    public void buttonResetPressed()
    {
        if (!_inWholeResetMode)
            simController.ResetSimulation();
        else
            simController.ResetWholeSimulation();
    }

    private void DeactivateResetButton()
    {
        if (!AllowWholeReset)
        {
            _canvasRenderer.SetAlpha(0.0f);
            _button.interactable = false;
        }
        else
            _inWholeResetMode = true;
    }

    private void ActivateResetButton()
    {
        if (!AllowWholeReset)
        {
            _canvasRenderer.SetAlpha(1.0f);
            _button.interactable = true;
        }
        else
            _inWholeResetMode = false;
    }
}
