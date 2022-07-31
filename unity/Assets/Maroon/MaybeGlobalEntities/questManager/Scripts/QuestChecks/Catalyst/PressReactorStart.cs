using System;
using Maroon.Chemistry.Catalyst;
using QuestManager;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class PressReactorStart : IQuestCheck
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
