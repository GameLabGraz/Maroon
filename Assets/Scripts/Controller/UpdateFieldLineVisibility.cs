//-----------------------------------------------------------------------------
// UpdateFieldLineVisibility.cs
//
// Script to update the field line visibility
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to update the field line visibility
/// </summary>
public class UpdateFieldLineVisibility : MonoBehaviour
{
    /// <summary>
    /// The field line visibility toggle
    /// </summary>
    private Toggle toggleFieldLineVisibility;

    /// <summary>
    /// Stores the field with the toggle game component
    /// </summary>
    private void Start()
    {
        toggleFieldLineVisibility = gameObject.GetComponent<Toggle>();
    }

    /// <summary>
    /// Sets the field lines visible or hidden
    /// </summary>
    /// <param name="flManager">The field line manager</param>
    public void SetVisibility(FieldLineManager flManager)
    {
        flManager.SetFieldLinesVisible(toggleFieldLineVisibility.isOn);
    }
}
