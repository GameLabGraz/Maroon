using System;
using UnityEngine;
using GameLabGraz.QuestManager;


namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class RingResistanceCheck : QuestCheck
    {
        private Coil _coil;
        private float _initResistancefactor;

        protected override void InitCheck()
        {
            _coil = FindObjectOfType<Coil>();
            if (_coil == null) throw new NullReferenceException("There is no coil in the scene.");

            _initResistancefactor = _coil.ResistanceFactor;
        }

        protected override bool CheckCompliance()
        {
            return Math.Abs(_initResistancefactor - _coil.ResistanceFactor) > 0.1f;
        }
    }
}
