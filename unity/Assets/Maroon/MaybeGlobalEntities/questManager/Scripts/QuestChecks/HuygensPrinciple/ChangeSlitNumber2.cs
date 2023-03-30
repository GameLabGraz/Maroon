using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ChangeSlitNumber2 : IQuestCheck
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