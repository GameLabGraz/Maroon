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
    private IField _field;
    private bool _isParentNull;

    private bool _hasScaling = false;
    private ScalingArrow _scalingArrow;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        _isParentNull = transform.parent == null; //init it here as this null look up is expensive in update
        // Field = GameObject.FindGameObjectWithTag("Field"); -> super inefficient if 20x20 arrows search for the same tag at once...
        Debug.Assert(Field != null);
        _field = Field.GetComponent<IField>();

        _scalingArrow = GetComponent<ScalingArrow>();
        _hasScaling = _scalingArrow != null;
        
        rotateArrow();
        start_rot = transform.rotation;
    }

    /// <summary>
    /// Update is called every frame and rotates the arrow
    /// </summary>
    private void Update()
    {
        if (!isActiveAndEnabled) return;
        if (OnlyUpdateInRunMode && !SimulationController.Instance.SimulationRunning) return;
        rotateArrow();
    }

    /// <summary>
    /// Rotates the arrow based on the field
    /// </summary>
    private void rotateArrow()
    {
        if (_hasScaling)
        {
            _scalingArrow.size = _field.getStrengthInPercent(transform.position) * 100f;

        }
        
        var rotate = _field.get(transform.position) * fieldStrengthFactor;
        
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
//        transform.localRotation = Quaternion.Euler(-90, rot, 0);
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        if(isActiveAndEnabled)
            rotateArrow();
    }
}
