//-----------------------------------------------------------------------------
// ArrowController.cs
//
// Controller class for field arrows
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
/// Controller class for field arrows
/// </summary>
public class ArrowController : MonoBehaviour, IResetObject
{
    /// <summary>
    /// The physical field
    /// </summary>
    public GameObject Field;
    
    /// <summary>
    /// The physical field
    /// </summary>
    public bool Allow3dRotation = false;

    /// <summary>
    /// The rotation offset
    /// </summary>
    public float rotationOffset = 0f;

    /// <summary>
    /// The field strength factor
    /// </summary>
    public float fieldStrengthFactor = Teal.FieldStrengthFactor;

    /// <summary>
    /// The start rotation for reseting
    /// </summary>
    public Quaternion start_rot;

    public bool OnlyUpdateInRunMode = true;

    private SimulationController simController;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        Field = GameObject.FindGameObjectWithTag("Field");
        if (Field != null)
            rotateArrow();
        start_rot = transform.rotation;

    }

    /// <summary>
    /// Update is called every frame and rotates the arrow
    /// </summary>
    void Update()
    {
        if (OnlyUpdateInRunMode && !simController.SimulationRunning)
            return;

        if (Field != null)
            rotateArrow();
        else
            Field = GameObject.FindGameObjectWithTag("Field");
    }

    /// <summary>
    /// Rotates the arrow based on the field
    /// </summary>
    private void rotateArrow()
    {
        Vector3 rotate = Field.GetComponent<IField>().get(transform.position) * fieldStrengthFactor;
        
        rotate.Normalize();
        float rot = 0;

        if (Allow3dRotation)
        {
            transform.up = rotate;
            return;
        }
        
        if(transform.parent != null)
        {
            if(transform.parent.rotation == Quaternion.Euler(-90, 0, 0))
                rot = Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(90, 0, 0))
                rot = -Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(90, 90, 0))
                rot = Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(90, -90, 0))
                rot = -Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(-90, 90, 0))
                rot = -Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;
            else if (transform.parent.rotation == Quaternion.Euler(-90, -90, 0))
                rot = Mathf.Atan2(rotate.z, rotate.y) * Mathf.Rad2Deg;

            else if (transform.parent.rotation == Quaternion.Euler(0, 0, 0))
                rot = Mathf.Atan2(rotate.x, rotate.z) * Mathf.Rad2Deg;
        }

        transform.localRotation = Quaternion.Euler(-90, rot, 0);
//        transform.localRotation = Quaternion.Euler(-90, rot, 0);
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        rotateArrow();
    }
}
