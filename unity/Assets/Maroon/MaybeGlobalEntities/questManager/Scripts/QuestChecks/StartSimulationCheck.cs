using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class StartSimulationCheck : IQuestCheck
    {
        private bool _wasChanged = false;

        protected override void InitCheck()
        {
            if (SimulationController.Instance == null) throw new NullReferenceException("There is no SimulationController in the scene.");
        }

        protected override bool CheckCompliance()
        {
            _wasChanged = _wasChanged || SimulationController.Instance.SimulationRunning;
            return _wasChanged;
        }
    }
}