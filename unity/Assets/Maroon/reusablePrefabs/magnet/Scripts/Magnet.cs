//-----------------------------------------------------------------------------
// Magnet.cs
//
// Class for a permanent magnet.
// Is also a electromagnetic object.  
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Class for a permanent magnet.
/// Is also a electromagnetic object.  
/// </summary>
public class Magnet : EMObject
{
    /// <summary>
    /// Sets the field strength factor.
    /// User can sets the strength factor by a slider.
    /// </summary>
    /// <param name="fieldStrength">The field strength to be set</param>
    public void SetFieldStrength(float fieldStrength)
    {
        FieldStrength = fieldStrength;
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
        if (forceActive)
        {
            GetComponent<Rigidbody>().AddForce(GetExternalForce()); 
        }
    }

    public Vector3 GetExternalForce()
    {
        return GameObject.Find("Coil").GetComponent<Coil>().GetExternalForce() * transform.up; //hack to get force from coil
    }

    protected override void HandleUpdate()
    {
        return;
    }
}
