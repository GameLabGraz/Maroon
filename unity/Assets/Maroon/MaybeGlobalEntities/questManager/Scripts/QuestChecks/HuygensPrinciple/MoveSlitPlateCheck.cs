using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MoveSlitPlateCheck : IQuestCheck
    {
        private SlitPlate _slitPlate;
        private Vector3 _initPosition;


        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();

            if (_slitPlate == null) throw new NullReferenceException("There is no slit plate in the scene.");

            _initPosition = _slitPlate.transform.position;

        }

        protected override bool CheckCompliance()
        {
            return Vector3.Distance(_initPosition, _slitPlate.transform.position) > 0.2f;
        }
    }
}