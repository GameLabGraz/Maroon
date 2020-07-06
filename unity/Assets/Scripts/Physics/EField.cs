//-----------------------------------------------------------------------------
// EField.cs
//
// Class to represent a electric field
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;

/// <summary>
/// Class to represent a electric field
/// </summary>
public class EField : IField
{
    /// <summary>
    /// Gets the field type
    /// </summary>
    /// <returns>The field type</returns>
	public override FieldType getFieldType()
    {
        return FieldType.EField;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake()
    {
        updateProducers();
    }

    /// <summary>
    /// Updates Producers, are there any other?
    /// </summary>
	public void updateProducers()
    {
        GameObject[] sensedObjects = GameObject.FindGameObjectsWithTag("GenerateE");
        foreach (GameObject currGO in sensedObjects)
        {
            if (currGO.transform.parent != null && currGO.transform.parent.gameObject != null)
                producers.Add(currGO.transform.parent.gameObject);
        }
    }

    /// <summary>
    /// Gets the combined electric field at a given position
    /// </summary>
    /// <param name="position">The required position</param>
    /// <returns>The electric field vector</returns>
    public override Vector3 get(Vector3 position)
    {
        Vector3 field = Vector3.zero;
        try
        {
            if (useCallback) producers = onGetProducers.Invoke();
            
            foreach (GameObject producer in producers)
            {
                if (producer.gameObject.activeSelf)
                {
                    field += producer.GetComponent<IGenerateE>().getE(position);
                }
            }
        }
        catch
        {
            updateProducers();
        }
        return field;
    }

    /// <summary>
    /// Gets the combined electric field at a given position excluded the given EM object.
    /// </summary>
    /// <param name="position">The required position</param>
    /// <param name="xobj">Ignored object</param>
    /// <returns>the electric field vector</returns>
    public override Vector3 get(Vector3 position, GameObject xobj)
    {
        Vector3 field = Vector3.zero;
        try
        {
            if (useCallback) producers = onGetProducers.Invoke();

            foreach (GameObject producer in producers)
            {
                if (producer.gameObject.activeSelf)
                {
                    if (producer != xobj)
                        field += producer.GetComponent<IGenerateE>().getE(position);
                }
            }
        }
        catch
        {
            updateProducers();
        }
        return field;
    }

    public override float getStrength(Vector3 position)
    {
        var strength = 0f;
        try
        {
            if (useCallback) producers = onGetProducers.Invoke();

            foreach (GameObject producer in producers)
            {
                if (producer.gameObject.activeSelf)
                {
                    strength += producer.GetComponent<IGenerateE>().getEPotential(position);
                }
            }
        }
        catch
        {
            updateProducers();
        }
        return strength;
    }

    public override float getStrengthInPercent(Vector3 position)
    {
        var original = getStrength(position);
        // Debug.Log("Original: " + original);
        var strength = Mathf.Clamp(Mathf.Abs(original), minimumStrength, maximumStrength);
        var strengthInRealPercent = strength / Mathf.Abs(maximumStrength - minimumStrength);
        // Debug.Log("Strength: " + strength);
        // Debug.Log("Strength in Percent: " + strengthInRealPercent);
        var onCurve = distributionCurveForStrength.Evaluate(strengthInRealPercent);
        // Debug.Log("OnCurve: " + onCurve);
        return onCurve; //distributionCurveForStrength.Evaluate(strengthInRealPercent);
    }
}
