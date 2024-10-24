using System;
using UnityEngine;
using GameLabGraz.QuestManager;


namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class RingResistanceCheck : QuestCheck
    {
        private Coil _coil;
        private float _initConductivity;

        protected override void InitCheck()
        {
            _coil = FindObjectOfType<Coil>();
            if (_coil == null) throw new NullReferenceException("There is no coil in the scene.");

            _initConductivity = _coil.conductivity;
        }

        protected override bool CheckCompliance()
        {
            return Math.Abs(_initConductivity - _coil.conductivity) > 0.1f;
        }
    }
}
