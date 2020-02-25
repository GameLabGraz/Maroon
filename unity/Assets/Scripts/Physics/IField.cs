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

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Field types
/// </summary>
public enum FieldType
{
    EField,
    BField
}

[Serializable]
public class ProducersCallback : SerializableCallback<HashSet<GameObject>> {}

/// <summary>
/// Interface to represent a physical field
/// </summary>
public abstract class IField : MonoBehaviour
{
    [Tooltip("If true then the callback is used to get the producers.")]
    public bool useCallback = false;
    
    [Tooltip("This method is only used when use Callback is set to true.")]
    public ProducersCallback onGetProducers;

    [Header("Resizing and Transparent Appearance Settings")]
    public float maximumStrength;
    public float minimumStrength;
    [Tooltip("Shows the distribution of the strengths.")]
    public AnimationCurve distributionCurveForStrength = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    /**
     * |    /
     * |  /      -> Curve: small x (= strength) have low strength factor (=y)
     * |/
     * -------
     */
        
    /// <summary>
    /// Lists of producers which generates the field
    /// </summary>
    protected HashSet<GameObject> producers = new HashSet<GameObject>();
        
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
    
    /// <summary>
    ///  Gets the combined field-strength at a given position.
    /// </summary>
    /// <param name="position">The required position</param>
    /// <returns>The field strength</returns>
    public abstract float getStrength(Vector3 position);

    public abstract float getStrengthInPercent(Vector3 position);
}
