using System;
using Maroon.Chemistry.Catalyst;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class IncreaseTemperature : QuestCheck
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
            return _catalystController.HasInitialTempChanged;
        }

    }
}