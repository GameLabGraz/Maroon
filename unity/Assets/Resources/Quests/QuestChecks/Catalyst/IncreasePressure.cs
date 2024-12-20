using System;
using Maroon.Chemistry.Catalyst;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class IncreasePressure : QuestCheck
    {
        private CatalystController _catalystController;

        private float _initialPartialPressure;
        
        protected override void InitCheck()
        {
            _catalystController = FindObjectOfType<CatalystController>();
            if (!_catalystController)
                throw new NullReferenceException("no catalyst controller in scene!");

            _initialPartialPressure = _catalystController.PartialPressure;

        }

        protected override bool CheckCompliance()
        {
            return Mathf.Abs(_catalystController.PartialPressure - _initialPartialPressure) > 
                   CatalystConstants.PartialPressureValues[(int)CatalystController.ExperimentVariation][1];
        }
    }
}