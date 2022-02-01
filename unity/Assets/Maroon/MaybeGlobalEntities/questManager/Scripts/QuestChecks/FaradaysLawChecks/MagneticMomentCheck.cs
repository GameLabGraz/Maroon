using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MagneticMomentCheck : IQuestCheck
    {
        private Magnet _magnet;
        private float _initFieldStrength;

        protected override void InitCheck()
        {
            _magnet = FindObjectOfType<Magnet>();
            if (_magnet == null) throw new NullReferenceException("There is no magnet in the scene.");

            _initFieldStrength = _magnet.FieldStrength;
        }

        protected override bool CheckCompliance()
        {
            return Math.Abs(_initFieldStrength - _magnet.FieldStrength) > 0.1f;
        }
    }
}
