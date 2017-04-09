//-----------------------------------------------------------------------------
// SimulationController.cs
//
// Script to update the number of field lines
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class to update the number of field lines
/// </summary>
[RequireComponent(typeof(Slider))]
public class UpdateNumFieldLines : MonoBehaviour
{
    /// <summary>
    /// The slider for the number of field lines
    /// </summary>
    private Slider slider;

    /// <summary>
    /// Stores the slider component in the slider field
    /// </summary>
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    /// <summary>
    /// Sets the number of field lines
    /// </summary>
    /// <param name="flManager">The field line manager</param>
    public void SetNumFieldLines(FieldLineManager flManager)
    {
        flManager.setSymmetryCount((int)slider.value);
    }
}
