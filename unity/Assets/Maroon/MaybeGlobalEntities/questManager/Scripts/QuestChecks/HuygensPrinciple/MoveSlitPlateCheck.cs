using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MoveSlitPlateCheck : IQuestCheck
    {
        private SlitPlate _slitPlate;
        private Vector3 _initSlitPlatePosition;
        private float targetDistance = 0.1f;

        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();
            if (_slitPlate == null) throw new NullReferenceException("There is no slit plate in the scene.");

            _initSlitPlatePosition = _slitPlate.transform.position;
        }

        protected override bool CheckCompliance()
        {
            return Vector3.Distance(_initSlitPlatePosition, _slitPlate.transform.position) > targetDistance;
        }
    }
}