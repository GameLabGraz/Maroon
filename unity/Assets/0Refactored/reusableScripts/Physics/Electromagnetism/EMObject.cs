//-----------------------------------------------------------------------------
// EMObject.cs
//
// Abstract base class for electro magnetic objects
//-----------------------------------------------------------------------------
//

using Maroon.Physics;
using UnityEngine;

/// <summary>
/// Abstract base class for electromagnetic objects
/// </summary>
public abstract class EMObject : PausableObject, IGenerateB, IResetObject
{
    public enum FieldAxis
    {
        Up, Right, Forward
    }

    /// <summary>
    /// The field strength factor
    /// </summary>
    public float field_strength = 0;

    public FieldAxis fieldAxis = FieldAxis.Up;
    
    /// <summary>
    /// Activated acting forces on the object
    /// </summary>
    public bool force_active = false;

    /// <summary>
    /// The start position of the object for reseting
    /// </summary>
    protected Vector3 startPos;

    /// <summary>
    /// The start rotation of the object for reseting
    /// </summary>
    protected Quaternion startRot;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public Vector3 FieldAlignment
    {
        get
        {
            switch (fieldAxis)
            {
                case FieldAxis.Up:
                    return transform.up;
                case FieldAxis.Right:
                    return transform.right;
                case FieldAxis.Forward:
                    return transform.forward;
                default:
                    return Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        base.Start();

        startPos = transform.position;
        startRot = transform.rotation;
    }

    /// <summary>
    /// Gets the dipole moment
    /// </summary>
    /// <returns>The dipole moment</returns>
    private Vector3 getDipoleMoment()
    {
        return FieldAlignment * field_strength;
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
        var n = position - transform.position;
        var m = getDipoleMoment();
        var r = n.magnitude; //length

        n.Normalize();
        n.Scale(new Vector3(3.0f * Vector3.Dot(m, n),
                            3.0f * Vector3.Dot(m, n),
                            3.0f * Vector3.Dot(m, n)));
        var B = n - m;

        B.Scale(new Vector3(1.0f / (r * r * r),
                            1.0f / (r * r * r),
                            1.0f / (r * r * r)));
        B.Scale(new Vector3(PhysicalConstants.erVacuumOver4Pi,
                            PhysicalConstants.erVacuumOver4Pi,
                            PhysicalConstants.erVacuumOver4Pi));
        return B;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public abstract void ResetObject();
}
