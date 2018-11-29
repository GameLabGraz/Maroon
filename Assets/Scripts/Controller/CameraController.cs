//-----------------------------------------------------------------------------
// CameraController.cs
//
// Controller class for the main camera
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controller class for the main camera
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The movement speed
    /// </summary>
    private int speed = 300;

    /// <summary>
    /// The mouse origin
    /// </summary>
    private Vector3 mouseOrigin;

    /// <summary>
    /// Indicates if the mouse is over a panel
    /// </summary>
    private bool mouseOverPanel = false;

    /// <summary>
    /// The origin position for reseting
    /// </summary>
    Vector3 origPos;

    /// <summary>
    /// The origin rotation for reseting
    /// </summary>
    Quaternion origRot;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        origPos = transform.position;
        origRot = transform.rotation;
    }

    /// <summary>
    /// Resets the camera
    /// </summary>
    public void ResetCamera()
    {
        transform.position = origPos;
        transform.rotation = origRot;
    }

    /// <summary>
    /// Sets the mouse over panel value
    /// </summary>
    /// <param name="mouseOverPanel">Mouse over panel value</param>
    public void setMouseOverPanel(bool mouseOverPanel)
    {
        this.mouseOverPanel = mouseOverPanel;
    }

    /// <summary>
    /// LateUpdate is called after all Update functions have been called.
    /// Rotates, moves and zooms
    /// </summary>
    void LateUpdate()
    {
        if (mouseOverPanel)
                return;

        if (Input.GetButton("Fire1"))
            rotateCamera();

        if (Input.GetButton("Fire2"))
            moveCamera();

        if (Input.GetButton("Fire3"))
            zoomCamera();

        mouseOrigin = Input.mousePosition;
    }

    /// <summary>
    /// Moves the camera
    /// </summary>
    private void moveCamera()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
        Vector3 move = new Vector3(pos.x * speed, pos.y * speed, 0);
        transform.Translate(move);
    }

    /// <summary>
    /// Rotates the camera
    /// </summary>
    private void rotateCamera()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
        transform.RotateAround(Vector3.zero, transform.right, -pos.y * speed);
        transform.RotateAround(Vector3.zero, Vector3.up, pos.x * speed);
    }

    /// <summary>
    /// Zomms in or out
    /// </summary>
    private void zoomCamera()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
        Vector3 zoom = new Vector3(0, 0, pos.y * speed);
        transform.Translate(zoom);
    }

  // OnGUI is called once per frame
  public void OnGUI()
  {
    // show controls on top left corner
    GUI.Label(new Rect(10f, 10f, 300f, 200f), string.Format("[ESC] - Leave"));
  }

}
