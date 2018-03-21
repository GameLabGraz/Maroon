//-----------------------------------------------------------------------------
// BField.cs
//
// Class to represent a magnetic field
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to represent a magnetic field
/// </summary>
public class BField : IField
{
    /// <summary>
    /// Lists of producers which generats a magnetic field
    /// </summary>
    private HashSet<GameObject> producers = new HashSet<GameObject>();

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
        GameObject[] sensedObjects = GameObject.FindGameObjectsWithTag("GenerateB");
        foreach (GameObject currGO in sensedObjects)
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
        Vector3 field = Vector3.zero;
        try
        {
            foreach (GameObject producer in producers)
            {
                if (producer.gameObject.activeSelf)
                {
                    field += producer.GetComponent<IGenerateB>().getB(position);
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
        Vector3 field = Vector3.zero;
        try
        {
            foreach (GameObject producer in producers)
            {
                if (producer.gameObject.activeSelf)
                {
                    if (producer != xobj)
                        field += producer.GetComponent<IGenerateB>().getB(position);
                }
            }
        }
        catch
        {
            updateProducers();
        }
        return field;
    }
}
