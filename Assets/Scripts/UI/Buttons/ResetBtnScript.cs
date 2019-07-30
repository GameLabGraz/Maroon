//-----------------------------------------------------------------------------
// ResetBtnScript.cs
//
// Script for reset button
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI.Buttons
{
    /// <summary>
    /// Script for reset button
    /// </summary>
    public class ResetBtnScript : BaseButton
    {
        protected override void Start()
        {
            base.Start();

            SimController.OnReset += OnResetHandler;
            SimController.OnStart += OnStartHandler;

            Disable();
        }

        private void OnStartHandler(object sender, EventArgs e)
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            gameObject.GetComponent<Button>().interactable = true;

        }

        private void OnResetHandler(object sender, EventArgs e)
        {
            gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
            gameObject.GetComponent<Button>().interactable = false;
        }

        /// <summary>
        /// Handles the button being pressed and resets the simulation
        /// </summary>
        public void ButtonResetPressed()
        {
            SimController.ResetSimulation();
        }
    }
}
