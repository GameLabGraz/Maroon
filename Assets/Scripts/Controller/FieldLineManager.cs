//-----------------------------------------------------------------------------
// FieldLineManager.cs
//
// Controller class to manage the field lines
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
using UnityEngine.UI;

/// <summary>
/// Controller class to manage the field lines
/// </summary>
public class FieldLineManager : MonoBehaviour
{
    /// <summary>
    /// The symmetry count, number of copys
    /// </summary>
    public int symmetryCount = 20;

    /// <summary>
    /// The symmetry axis
    /// </summary>
    public Vector3 symmetryAxis;

    /// <summary>
    /// Lists of field lines
    /// </summary>
    private HashSet<FieldLine> fieldLines;

    /// <summary>
    /// Indicates if symmetry is enabled
    /// </summary>
    private bool symmetryEnabled = true;

    /// <summary>
    /// The draw count
    /// </summary>
    private int drawCounter = 0;

    /// <summary>
    /// Initialization
    /// </summary>
    public void Awake()
    {
        fieldLines = new HashSet<FieldLine>();
    }

    /// <summary>
    /// Initialization
    /// </summary>
    public void Start()
    {
        GameObject[] sensedObjects = GameObject.FindGameObjectsWithTag("FieldLine");

        foreach (GameObject sensedTag in sensedObjects)
        {
            GameObject parent = sensedTag.transform.parent.gameObject;
            addFieldLine(parent.GetComponent<FieldLine>());
        }
    }

    /// <summary>
    /// Sets the field line visibility
    /// </summary>
    /// <param name="visibility">The visibility value</param>
    public void setFieldLinesVisible(bool visibility)
    {
        foreach (FieldLine fL in fieldLines)
        {
            fL.setVisibility(visibility);
        }
    }

    /// <summary>
    /// Updates the symmetry
    /// </summary>
    private void updateSymmetry()
    {
        foreach (FieldLine fL in fieldLines)
        {
            fL.setSymmetry(symmetryCount, symmetryAxis);
        }
    }

    /// <summary>
    /// Sets the symmetry count
    /// </summary>
    /// <param name="value"></param>
    public void setSymmetryCount(int value)
    {
        Debug.Log("FieldLines: " + value);
        int symmetryCnt = value;

        symmetryEnabled = symmetryCnt == 1 ? false : true;
        symmetryCount = symmetryCnt;
        updateSymmetry();
    }

    /// <summary>
    /// Adds the given field line to list
    /// </summary>
    /// <param name="fL">The field line</param>
    public void addFieldLine(FieldLine fL)
    {
        fieldLines.Add(fL);
        if (symmetryEnabled)
            fL.setSymmetry(symmetryCount, symmetryAxis);
    }

    /// <summary>
    /// Removes the given field line from list
    /// </summary>
    /// <param name="fL">The field line</param>
    public void removeFieldLine(FieldLine fL)
    {
        fieldLines.Remove(fL);
    }

    /// <summary>
    /// This function is called every fixed framerate frame and draw fild lines
    /// </summary>
    public void FixedUpdate()
    {
        drawCounter++;
        if ((drawCounter % Teal.FieldLineDrawDivisor) != 0)
            return;
        drawCounter = 1;

        if(fieldLines == null)
            return;
        foreach (FieldLine fl in fieldLines)
            fl.draw();
    }
}
