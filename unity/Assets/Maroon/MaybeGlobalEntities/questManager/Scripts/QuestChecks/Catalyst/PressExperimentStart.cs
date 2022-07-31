using System;
using QuestManager;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class PressExperimentStart : IQuestCheck
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