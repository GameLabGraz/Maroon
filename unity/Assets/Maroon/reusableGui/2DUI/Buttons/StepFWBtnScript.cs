//-----------------------------------------------------------------------------
// StepFWBtnScript.cs
//
// Script for forward button
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;

namespace Maroon.UI.Buttons
{
    /// <summary>
    /// Script for forward button
    /// </summary>
    public class StepFWBtnScript : BaseButton
    {
        protected override void Start()
        {
            base.Start();
            SimulationController.Instance.OnStart.AddListener(Disable);
            SimulationController.Instance.OnStop.AddListener(Enable);
        }

        /// <summary>
        /// Handles the button being pressed
        /// </summary>
        public void StepFwButtonPressed()
        {
            SimulationController.Instance.SimulateStep();
        }
    }
}
