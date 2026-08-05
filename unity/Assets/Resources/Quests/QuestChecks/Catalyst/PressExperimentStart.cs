using System;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class PressExperimentStart : QuestCheck
    {
        private SimulationController _simulationController;

        protected override void InitCheck()
        {
            _simulationController = FindObjectOfType<SimulationController>();
            if (!_simulationController)
                throw new NullReferenceException("no simulation controller in scene!");
        }

        protected override bool CheckCompliance()
        {
            return _simulationController.SimulationRunning;
        }

    }
}