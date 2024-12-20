using System;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class StartSimulationCheck : QuestCheck
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