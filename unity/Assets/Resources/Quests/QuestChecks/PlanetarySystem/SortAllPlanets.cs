using System;
using GameLabGraz.QuestManager;
using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem.Quests
{
    [RequireComponent(typeof(Quest))]
    public class SortAllPlanets : QuestCheck
    {
        private PlanetaryControllerVR _pc;

        protected override void InitCheck()
        {
            _pc = PlanetaryControllerVR.Instance;
            if (_pc == null)
                throw new NullReferenceException("No PlanetaryController in scene!");
            Debug.Log($"[SortAllPlanets] startCount={_pc.sortedPlanetCount}");
        }

        protected override bool CheckCompliance()
        {
            bool ok = _pc.sortedPlanetCount >= 10;
            return ok;
        }
    }
}
