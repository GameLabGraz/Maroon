//-----------------------------------------------------------------------------
// MagnetController.cs
//
// Controller class for the magnet
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
/// Controller class for the magnet
/// </summary>
public class MagnetController : MonoBehaviour
{
    /// <summary>
    /// The magnet movement speed
    /// </summary>
    public float magnetSpeed = 20.0f;

    /// <summary>
    /// Indicates if the mouse is over a panel
    /// </summary>
    private bool mouseOverPanel = false;

    /// <summary>
    /// Sets the mouse over panel value
    /// </summary>
    /// <param name="mouseOverPanel">Mouse over panel value</param>
    public void setMouseOverPanel(bool mouseOverPanel)
    {
        this.mouseOverPanel = mouseOverPanel;
    }

    /// <summary>
    /// Moves the magnet depending on the mouse movement
    /// Is called when the user has clicked and is still holding down the mouse.
    /// </summary>
    public void OnMouseDrag()
    {
        if (!SimController.Instance.SimulationRunning || mouseOverPanel)
            return;

        Vector3 mousePosScreen = Input.mousePosition;
        mousePosScreen.z = magnetSpeed;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosScreen);

        Vector3 target = transform.position;
        target.x = mousePos.x;

        transform.position = Vector3.MoveTowards(transform.position, target, 10.0f);
    }
}
