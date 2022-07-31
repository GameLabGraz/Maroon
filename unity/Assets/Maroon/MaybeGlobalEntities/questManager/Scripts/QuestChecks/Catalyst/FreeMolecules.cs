using System;
using GameLabGraz.VRInteraction;
using Maroon.Chemistry.Catalyst;
using QuestManager;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class FreeMolecules : IQuestCheck
    {
        private CatalystController _catalystController;
        protected override void InitCheck()
        {
            _catalystController = FindObjectOfType<CatalystController>();
            if (!_catalystController)
                throw new NullReferenceException("no catalyst controller in scene!");
        }

        protected override bool CheckCompliance()
        {
            return _catalystController.AreHinshelwoodMoleculesFreed;
        }

    }
}