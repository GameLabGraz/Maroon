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

/// <summary>
/// Interface for objects which must be reset.
/// </summary>
public interface IResetWholeObject : IResetObject
{
    /// <summary>
    /// Resets the object
    /// </summary>
    void ResetWholeObject();
}
