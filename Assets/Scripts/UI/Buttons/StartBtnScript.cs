//-----------------------------------------------------------------------------
// StartBtnScript.cs
//
// Script for start button
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
    /// Script for start button
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class StartBtnScript : BaseButton
    {
        /// <summary>
        /// Icon of button when simulation is paused
        /// </summary>
        [SerializeField]
        private Sprite _playIcon = null;

        /// <summary>
        /// Icon of button when simulation is running
        /// </summary>
        [SerializeField]
        private Sprite _pauseIcon = null;
        

        protected  override void Start()
        {
            base.Start();
            SimulationController.Instance.OnStart += OnStartHandler;
            SimulationController.Instance.OnStop += OnStopHandler;
        }

        private void OnStartHandler(object sender, EventArgs e)
        {
            Button.image.sprite = _pauseIcon;
        }

        private void OnStopHandler(object sender, EventArgs e)
        {
            Button.image.sprite = _playIcon;
        }

        /// <summary>
        /// Handles the button being pressed
        /// </summary>
        public void ButtonStartPressed()
        {
            if (SimulationController.Instance.SimulationRunning)
                SimulationController.Instance.StopSimulation();
            else
                SimulationController.Instance.StartSimulation();
        }
    }
}

