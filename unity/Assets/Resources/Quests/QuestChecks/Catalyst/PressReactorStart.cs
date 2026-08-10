using System;
using Maroon.Chemistry.Catalyst;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class PressReactorStart : QuestCheck
    {
        private CatalystReactor _catalystReactor;
        protected override void InitCheck()
        {
            _catalystReactor = FindObjectOfType<CatalystReactor>();
            if (!_catalystReactor)
                throw new NullReferenceException("no catalyst reactor in scene!");
        }

        protected override bool CheckCompliance()
        {
            return _catalystReactor.ReactionStarted;
        }
    }
}
