//-----------------------------------------------------------------------------
// IGenerateB.cs
//
// Interface for objects which generates a magnetic field.
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
/// Interface for objects which generates a magnetic field.
/// </summary>
public interface IGenerateB
{
    /// <summary>
    /// Gets the magnetic field at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	Vector3 getB(Vector3 position);

    float getFieldStrength();
}
