using Maroon.Physics.HuygensPrinciple;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class ChangeSlitWidthCheck : QuestCheck
    {
        private SlitPlate _slitPlate;
        private float _initSlitWidth;
        private readonly float _targetDifference = 0.1f;

        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();
            _initSlitWidth = _slitPlate.SlitWidth;
        }

        protected override bool CheckCompliance()
        {
            return Mathf.Abs(_initSlitWidth - _slitPlate.SlitWidth) > _targetDifference;
        }
    }
}