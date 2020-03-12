using System;

namespace QuestManager
{
    public class RingResistanceCheck : IQuestCheck
    {
        private Coil _coil;
        private float _initResistancefactor;

        protected override void InitCheck()
        {
            _coil = FindObjectOfType<Coil>();
            if (_coil == null) throw new NullReferenceException("There is no coil in the scene.");

            _initResistancefactor = _coil.GetResistanceFactor();
        }

        protected override bool CheckCompliance()
        {
            return Math.Abs(_initResistancefactor - _coil.GetResistanceFactor()) > 0.1f;
        }
    }
}
