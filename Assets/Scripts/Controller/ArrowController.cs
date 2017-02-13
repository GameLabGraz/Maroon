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

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
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
        if (!SimController.Instance.SimulationRunning)
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
        float rotZ = -Mathf.Atan2(rotate.x, rotate.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void resetObject()
    {
        rotateArrow();
    }
}
