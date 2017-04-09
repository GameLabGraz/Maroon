//-----------------------------------------------------------------------------
// Teal.cs
//
// Class containing important constants
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
/// Class containing important constants
/// </summary>
public static class Teal
{

    /// <summary>
    /// Time Scale for running simulation
    /// </summary>
    public static readonly float TimeScaleSimulationRunning = 1;//0.3f;

    /// <summary>
    /// The permitivity of vacuum
    /// </summary>
    public static readonly float PermitivityOfVacuum = 1;

    /// <summary>
    /// The permitivity of vacuum over 4pi
    /// </summary>
	public static readonly float PermitivityVacuumOver4Pi = PermitivityOfVacuum / (4.0f * Mathf.PI);

    /// <summary>
    /// The default number of field lines
    /// </summary>
    public static readonly int DefaultNumFieldLines = 20;

    /// <summary>
    /// The field line draw divisor
    /// </summary>
    public static readonly int FieldLineDrawDivisor = 2;

    /// <summary>
    /// The field line vertex count
    /// </summary>
    public static readonly int FieldLineVertexCount = 300;

    /// <summary>
    /// The field line strength factor
    /// </summary>
    public static readonly int FieldStrengthFactor = 100;
}
