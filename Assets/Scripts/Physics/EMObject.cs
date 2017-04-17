//-----------------------------------------------------------------------------
// EMObject.cs
//
// Abstract base class for electro magnetic objects
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
/// Abstract base class for electro magnetic objects
/// </summary>
public abstract class EMObject : PausableObject, IGenerateB, IResetObject
{
    /// <summary>
    /// Activated acting forces on the object
    /// </summary>
    public bool force_active = false;

    /// <summary>
    /// The field strength factor
    /// </summary>
    public float field_strength = 0;

    /// <summary>
    /// The start position of the object for reseting
    /// </summary>
    protected Vector3 startPos;

    /// <summary>
    /// The start rotation of the object for reseting
    /// </summary>
    protected Quaternion startRot;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    /// <summary>
    /// Gets the dipole moment
    /// </summary>
    /// <returns>The dipole moment</returns>
    private Vector3 getDipoleMoment()
    {
        Vector3 direction = transform.up;
        Vector3 dipolMoment = direction * field_strength;
        return dipolMoment;
    }

    /// <summary>
    /// Gets the field strength factor
    /// </summary>
    /// <returns>The field strength</returns>
    public float getFieldStrength()
    {
        return this.field_strength;
    }

    /// <summary>
    /// Gets the magnetic field at a given position
    /// </summary>
    /// <param name="position">The required position</param>
    /// <returns>The magnetic field vector at the position</returns>
    public Vector3 getB(Vector3 position)
    {
        //http://magician.ucsd.edu/Essentials_2/WebBook2ch1.html
        Vector3 B;
        Vector3 n = position - transform.position;
        Vector3 m = getDipoleMoment();
        float r = n.magnitude; //length

        n.Normalize();
        n.Scale(new Vector3(3.0f * Vector3.Dot(m, n),
                            3.0f * Vector3.Dot(m, n),
                            3.0f * Vector3.Dot(m, n)));
        B = n - m;

        B.Scale(new Vector3(1.0f / (r * r * r),
                            1.0f / (r * r * r),
                            1.0f / (r * r * r)));
        B.Scale(new Vector3(Teal.PermitivityVacuumOver4Pi,
                            Teal.PermitivityVacuumOver4Pi,
                            Teal.PermitivityVacuumOver4Pi));
        return B;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public abstract void resetObject();
}
