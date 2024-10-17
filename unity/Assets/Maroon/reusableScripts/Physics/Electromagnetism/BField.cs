//-----------------------------------------------------------------------------
// BField.cs
//
// Class to represent a magnetic field
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Linq;

/// <summary>
/// Class to represent a magnetic field
/// </summary>
public class BField : IField
{
    /// <summary>
    /// Gets the field type
    /// </summary>
    /// <returns></returns>
	public override FieldType getFieldType()
    {
        return FieldType.BField;
    }

    /// <summary>
    /// Initialization
    /// </summary>
	private void Awake()
    {
        updateProducers();
    }

    /// <summary>
    /// Updates Producers, are there any other?
    /// </summary>
    public void updateProducers()
    {
        var sensedObjects = GameObject.FindGameObjectsWithTag("GenerateB");
        foreach (var currGO in sensedObjects)
        {
            if (currGO.transform.parent != null && currGO.transform.parent.gameObject != null)
                producers.Add(currGO.transform.parent.gameObject);
        }
    }

    /// <summary>
    /// Gets the combined magnetic field at a given position
    /// </summary>
    /// <param name="position">The required position</param>
    /// <returns>The magnetic field vector</returns>
    public override Vector3 get(Vector3 position)
    {
        var field = Vector3.zero;
        try
        {
            if (useCallback) producers = onGetProducers.Invoke();

            foreach (var producer in producers.Select(p => p.GetComponent<IGenerateB>()))
            {
                if (producer != null && producer.Enabled)
                {
                    field += producer.GetB(position);
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
    /// Gets the combined magnetic field at a given position excluded the given EM object.
    /// </summary>
    /// <param name="position">The required position</param>
    /// <param name="xobj">Ignored object</param>
    /// <returns>The magnetic field vector</returns>
    public override Vector3 get(Vector3 position, GameObject xobj)
    {
        var field = Vector3.zero;
        try
        {
            if (useCallback) producers = onGetProducers.Invoke();

            foreach (var producer in producers.Select(p => p.GetComponent<IGenerateB>()))
            {
                if (producer != null && producer.Enabled)
                {
                    if (((Component)producer).gameObject != xobj)
                        field += producer.GetB(position);
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
        throw new System.NotImplementedException();
    }

    public override float getStrengthInPercent(Vector3 position)
    {
        return 1f;
    }

    public override void addProducerToSet(GameObject producer)
    {
        throw new System.NotImplementedException();
    }

    public override void removeProducerFromSet(GameObject producer)
    {
        throw new System.NotImplementedException();
    }
}
