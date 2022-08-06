using System;
using Maroon.Chemistry.Catalyst;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class IncreasePressure : IQuestCheck
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
            return _catalystController.HasInitialPressureChanged;
        }

    }
}