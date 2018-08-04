//-----------------------------------------------------------------------------
// IGenerateE.cs
//
// Interface for objects which generates a electric field.
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
/// Interface for objects which generates a electric field.
/// </summary>
public interface IGenerateE
{
    /// <summary>
    /// Gets the electric field at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	Vector3 getE(Vector3 position);

    /// <summary>
    /// Gets the flux at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	float getEFlux(Vector3 position);

    /// <summary>
    /// Gets the electrical potential at a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	float getEPotential(Vector3 position);

    float getFieldStrength();
}
