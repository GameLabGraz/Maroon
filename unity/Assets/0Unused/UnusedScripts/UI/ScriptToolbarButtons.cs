//-----------------------------------------------------------------------------
// ScriptToolbarButtons.cs
//
// Script to handle the toolbar buttons
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script to handle the toolbar buttons
/// </summary>
public class ScriptToolbarButtons : MonoBehaviour
{
    /// <summary>
    /// The file menu
    /// </summary>
	private static GameObject fileMenu = null;

    /// <summary>
    /// The view menu
    /// </summary>
	private static GameObject viewMenu = null;

    /// <summary>
    /// the help menu
    /// </summary>
	private static GameObject helpMenu = null;

    /// <summary>
    /// Initialization of members
    /// </summary>
    void Awake()
    {
        if (fileMenu == null)
            fileMenu = GameObject.Find("PanelFileMenu");
        if (viewMenu == null)
            viewMenu = GameObject.Find("PanelViewMenu");
        if (helpMenu == null)
            helpMenu = GameObject.Find("PanelHelpMenu");
    }

    /// <summary>
    /// Sets all menus inactive at start
    /// </summary>
    void Start()
    {
        fileMenu.SetActive(false);
        viewMenu.SetActive(false);
        helpMenu.SetActive(false);
    }

    /// <summary>
    /// Handles the file button being pressed and sets 
    /// the file menu active
    /// </summary>
    public void fileButtonPressed()
    {
        Debug.Log("PressedFile pressed");
        fileMenu.SetActive(!fileMenu.activeSelf);
        viewMenu.SetActive(false);
        helpMenu.SetActive(false);
    }

    /// <summary>
    /// Handles the view button being pressed and 
    /// sets the view menu active
    /// </summary>
    public void viewButtonPressed()
    {
        Debug.Log("PressedView pressed");
        fileMenu.SetActive(false);
        viewMenu.SetActive(!viewMenu.activeSelf);
        helpMenu.SetActive(false);
    }

    /// <summary>
    /// Handles the help button being pressed and
    /// sets the help menu active
    /// </summary>
    public void helpButtonPressed()
    {
        Debug.Log("PressedHelp pressed");
        fileMenu.SetActive(false);
        viewMenu.SetActive(false);
        helpMenu.SetActive(!helpMenu.activeSelf);
    }

    /// <summary>
    /// Handles the exit button being pressed and 
    /// quits the application.
    /// </summary>
    public void exitButtonPressed()
    {
        Application.Quit();
    }

    /// <summary>
    /// Handles the falling magnet button being pressed
    /// and loads the falling magnet scene
    /// </summary>
    public void loadFallingMagnetButtonPressed()
    {
        SceneManager.LoadScene("FallingMagnet");
    }

    /// <summary>
    /// Handles the falling coil button being pressed
    /// and loads the falling coil scene.
    /// </summary>
    public void loadFallingCoilButtonPressed()
    {
        SceneManager.LoadScene("FallingCoil");
    }

    /// <summary>
    /// Handles the faraday's law button being pressed
    /// and loads the farada's law scene
    /// </summary>
    public void loadFaradaysLawButtonPressed()
    {
        SceneManager.LoadScene("FaradaysLaw");
    }

}
