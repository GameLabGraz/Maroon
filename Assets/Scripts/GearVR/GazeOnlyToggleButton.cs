using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GazeOnlyToggleButton : GazeOnly_SelectionRadial
{
    public static event Action VR_ControlToggled;                                                // This event is triggered if GazeOnlyControlflag in GazeOnly_SelectionRadial.cs is changed.

    override public void OnEnable()
    {
        m_VRInteractiveItem.OnOver += HandleDown;
        m_VRInteractiveItem.OnOut += HandleUp;
        OnSelectionComplete += toggleGazeInput;
    }

    override public void OnDisable()
    {
        m_VRInteractiveItem.OnOver -= HandleDown;
        m_VRInteractiveItem.OnOut -= HandleUp;
        OnSelectionComplete -= toggleGazeInput;
    }

    private void toggleGazeInput()
    {
        gazeOnlyControl = !gazeOnlyControl;

        if (VR_ControlToggled != null)
            VR_ControlToggled();
    }
}
