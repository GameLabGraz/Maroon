//-----------------------------------------------------------------------------
// IGenerateB.cs
//
// Interface for objects which generates a magnetic field.
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Interface for objects which generates a magnetic field.
/// </summary>
public interface IGenerateB
{
    bool Enabled { get; set; }

    /// <summary>
    /// Gets the magnetic field at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	Vector3 GetB(Vector3 position);

    float GetFieldStrength();
}
