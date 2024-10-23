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

using Maroon.Physics;
using Maroon.Extensions;
using UnityEngine;

/// <summary>
/// Controller class for field arrows
/// </summary>
public class ArrowController : PausableObject, IResetObject
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
    public float fieldStrengthFactor = PhysicalConstants.FieldStrengthFactor;

    /// <summary>
    /// The start rotation for reseting
    /// </summary>
    public Quaternion start_rot;

    public bool OnlyUpdateInRunMode = true;
    private IField _field;
    private bool _isParentNull;

    private bool _hasScaling = false;
    private ScalingArrow _scalingArrow;

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        base.Start();

        _isParentNull = transform.parent == null; //init it here as this null look up is expensive in update
        // Field = GameObject.FindGameObjectWithTag("Field"); -> super inefficient if 20x20 arrows search for the same tag at once...
        Debug.Assert(Field != null);
        _field = Field.GetComponent<IField>();

        _scalingArrow = GetComponent<ScalingArrow>();
        _hasScaling = _scalingArrow != null;
        
        RotateArrow();
        start_rot = transform.rotation;
    }

    /// <summary>
    /// Update is called every frame and rotates the arrow
    /// </summary>
    protected override void Update()
    {
        if (!OnlyUpdateInRunMode) // Coulombs Law Hack 
            RotateArrow();
    }

    protected override void HandleUpdate()
    {

    }

    protected override void HandleFixedUpdate()
    {
        RotateArrow();
    }

    /// <summary>
    /// Rotates the arrow based on the field
    /// </summary>
    private void RotateArrow()
    {
        if (_field == null) return;

        if (_hasScaling)
        {
            _scalingArrow.size = _field.getStrengthInPercent(gameObject.SystemPosition()) * 100f;

        }

        var rotate = _field.get(gameObject.SystemPosition()) * fieldStrengthFactor;
        
        rotate.Normalize();
        float rot = 0;

        if (Allow3dRotation)
        {
            transform.up = rotate;
            return;
        }
        
        if(!_isParentNull)
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
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        if(isActiveAndEnabled)
            RotateArrow();
    }
}
