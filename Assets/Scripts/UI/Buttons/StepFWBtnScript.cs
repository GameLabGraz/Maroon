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
            SimController.OnStart += OnStartHandler;
            SimController.OnStop += OnStopHandler;
        }

        private void OnStartHandler(object sender, EventArgs e)
        {
            Disable();
        }

        private void OnStopHandler(object sender, EventArgs e)
        {
            Enable();
        }

        /// <summary>
        /// Handles the button being pressed
        /// </summary>
        public void StepFwButtonPressed()
        {
            SimController.SimulateStep();
        }
    }
}
