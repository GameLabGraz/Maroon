//-----------------------------------------------------------------------------
// IField.cs
//
// Interface to represent a physical field
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Field types
/// </summary>
public enum FieldType
{
    EField,
    BField
}

/// <summary>
/// Interface to represent a physical field
/// </summary>
public abstract class IField : MonoBehaviour
{
    /// <summary>
    /// Gets the field type
    /// </summary>
    /// <returns></returns>
    public abstract FieldType getFieldType();

    /// <summary>
    /// Gets the combined field at a given position
    /// </summary>
    /// <param name="position">The required positio</param>
    /// <returns>The field vector</returns>
    public abstract Vector3 get(Vector3 position);

    /// <summary>
    ///  Gets the combined field at a given position excluded the given EM object.
    /// </summary>
    /// <param name="position">The required position</param>
    /// <param name="xobj">Ignored object</param>
    /// <returns>The field vector</returns>
    public abstract Vector3 get(Vector3 position, GameObject xobj);
}
