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

        private float _initialTemperature;

        protected override void InitCheck()
        {
            _catalystController = FindObjectOfType<CatalystController>();
            if (!_catalystController)
                throw new NullReferenceException("no catalyst controller in scene!");

            _initialTemperature = _catalystController.Temperature;
        }

        protected override bool CheckCompliance()
        {
            return Mathf.Abs(_catalystController.Temperature - _initialTemperature) > 20.0f;
        }

    }
}