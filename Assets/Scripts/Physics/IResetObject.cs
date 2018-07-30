//-----------------------------------------------------------------------------
// IResetObject.cs
//
// Interface for objects which must be reset.
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
/// Interface for objects which must be reset.
/// </summary>
public interface IResetObject
{
    /// <summary>
    /// Resets the object
    /// </summary>
    void resetObject();
}
