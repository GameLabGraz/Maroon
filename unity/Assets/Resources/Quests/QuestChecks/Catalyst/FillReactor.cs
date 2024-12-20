using System;
using Maroon.Chemistry.Catalyst;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class FillReactor : QuestCheck
    {
        private CatalystReactor _reactor;
        private bool _reactorFilled;
    
        protected override void InitCheck()
        {
            _reactor = FindObjectOfType<CatalystReactor>();
            _reactorFilled = false;
            if (!_reactor)
                throw new NullReferenceException("no catalyst reactor in scene!");
        }

        protected override bool CheckCompliance()
        {
            if (_reactor.ReactorFilled)
                _reactorFilled = true;
            return _reactorFilled || _reactor.ReactorFilled;
        }
    }
}
