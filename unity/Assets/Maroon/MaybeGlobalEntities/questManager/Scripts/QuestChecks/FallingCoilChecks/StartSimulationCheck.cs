using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class StartSimulationCheck : IQuestCheck
    {
        private SimulationController _simulationController;
        private bool _initSimulationRunning;

        protected override void InitCheck()
        {
            _simulationController = FindObjectOfType<SimulationController>();
            if (_simulationController == null) throw new NullReferenceException("There is no SimulationController in the scene.");

            _initSimulationRunning = false;
        }

        protected override bool CheckCompliance()
        {
            return _initSimulationRunning != _simulationController.SimulationRunning &&
                   _simulationController.SimulationRunning;
        }
    }
}
