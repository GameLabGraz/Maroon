using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class ChangeSlitNumber2 : QuestCheck
    {
        private SlitPlate _slitPlate;
        
        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();
            if (_slitPlate == null) throw new NullReferenceException("There is no slit plate in the scene.");
        }

        protected override bool CheckCompliance()
        {
            return _slitPlate.NumberOfSlits == 2;
        }
    }
}