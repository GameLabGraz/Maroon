using System;
using Maroon.Physics.HuygensPrinciple;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class SlitPlateCheck : IQuestCheck
    {
        private SlitPlate _slitPlate;
        private int _initNumberOfSlits;
        private float _initSlitWidth;


        protected override void InitCheck()
        {
            _slitPlate = FindObjectOfType<SlitPlate>();

            if (_slitPlate == null) throw new NullReferenceException("There is no slit plate in the scene.");

            _initNumberOfSlits = _slitPlate.NumberOfSlits;
            _initSlitWidth = _slitPlate.SlitWidth;
        }

        protected override bool CheckCompliance()
        {
            return _initNumberOfSlits != _slitPlate.NumberOfSlits || Math.Abs(_initSlitWidth - _slitPlate.SlitWidth) > Mathf.Epsilon;
        }
    }
}