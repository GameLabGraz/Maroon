using System;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class FieldLineNumberCheck : QuestCheck
    {
        private SymmetricFieldLineManager _fieldLineManager;
        private int _initFieldLineNumber;

        protected override void InitCheck()
        {
            _fieldLineManager = FindObjectOfType<SymmetricFieldLineManager>();
            if (_fieldLineManager == null) throw new NullReferenceException("There is no FieldLineManager in the scene.");

            _initFieldLineNumber = _fieldLineManager.GetSymmetryCount();
        }

        protected override bool CheckCompliance()
        {
            return _initFieldLineNumber != _fieldLineManager.GetSymmetryCount();
        }
    }
}
