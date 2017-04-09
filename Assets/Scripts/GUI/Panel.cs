//-----------------------------------------------------------------------------
// Panel.cs
//
// Script to manage panels
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
/// Script to manage panels
/// </summary>
public class Panel : MonoBehaviour
{
    /// <summary>
    /// Sets active panel
    /// </summary>
    /// <param name="active_toggle">Toggle which sets panel active</param>
    public void setActivePanel(Toggle active_toggle)
    {
        gameObject.SetActive(active_toggle.isOn);
    }
}
