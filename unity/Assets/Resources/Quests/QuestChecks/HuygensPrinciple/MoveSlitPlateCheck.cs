using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class MoveSlitPlateCheck : QuestCheck
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