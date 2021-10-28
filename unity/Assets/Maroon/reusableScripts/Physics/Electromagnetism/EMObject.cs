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
    [SerializeField] protected float fieldStrength = 0;

    [SerializeField] protected FieldAxis fieldAxis = FieldAxis.Up;

    /// <summary>
    /// Activated acting forces on the object
    /// </summary>
    [SerializeField] protected bool forceActive = false;

    /// <summary>
    /// The start position of the object for reseting
    /// </summary>
    protected Vector3 startPos;

    /// <summary>
    /// The start rotation of the object for reseting
    /// </summary>
    protected Quaternion startRot;

    public float FieldStrength
    {
        set => fieldStrength = value;
        get => fieldStrength;
    }

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
    private Vector3 GetDipoleMoment()
    {
        return FieldAlignment * fieldStrength;
    }

    /// <summary>
    /// Gets the magnetic field at a given position
    /// </summary>
    /// <param name="position">The required position</param>
    /// <returns>The magnetic field vector at the position</returns>
    public Vector3 GetB(Vector3 position)
    {
        var n = position - transform.position;
        var m = GetDipoleMoment();
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

    public float GetFieldStrength()
    {
        return FieldStrength;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public abstract void ResetObject();
}
