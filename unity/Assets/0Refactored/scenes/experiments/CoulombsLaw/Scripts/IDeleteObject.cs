using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for objects which can be deleted when entering a gameobject's collider that has a DeleteOnCollision script attached.
/// </summary>
public interface IDeleteObject
{
    /// <summary>
    /// Resets the object
    /// </summary>
    void OnDeleteObject();
}

