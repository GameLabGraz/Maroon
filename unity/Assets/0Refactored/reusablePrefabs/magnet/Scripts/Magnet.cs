//-----------------------------------------------------------------------------
// Magnet.cs
//
// Class for a permanent magnet.
// Is also a electro magnetic object.  
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Class for a permanent magnet.
/// Is also a electro magnetic object.  
/// </summary>
public class Magnet : EMObject
{
    /// <summary>
    /// Sets the field strength factor.
    /// User can sets the strength factor by a slider.
    /// </summary>
    /// <param name="field_strength">The silder to get selected value from user</param>
    public void setFieldStrength(float field_strength)
    {
        this.field_strength = field_strength;
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public override void ResetObject()
    {
        var rg = GetComponent<Rigidbody>();
        if (rg)
        {
            rg.velocity = Vector3.zero;
            rg.angularVelocity = Vector3.zero;
        }
        transform.position = startPos;
        transform.rotation = startRot;
    }

    /// <summary>
    /// This function is called every fixed framerate frame and adds the force to the rigidbody
    /// if force effects are active.
    /// </summary>
    protected override void HandleFixedUpdate()
    {
        if (force_active)
        {
            GetComponent<Rigidbody>().AddForce(getExternalForce()); 
        }
    }

    public Vector3 getExternalForce()
    {
        return GameObject.Find("Coil").GetComponent<Coil>().getExternalForce() * transform.up; //hack to get force from coil
    }

    protected override void HandleUpdate()
    {
        return;
    }
}
