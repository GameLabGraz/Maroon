using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class IronFillingCheck : IQuestCheck
    {
        private IronFiling _ironFiling;

        protected override void InitCheck()
        {
            _ironFiling = FindObjectOfType<IronFiling>();
        }

        protected override bool CheckCompliance()
        {
            if (_ironFiling != null) return _ironFiling.gameObject.activeSelf;
            
            _ironFiling = FindObjectOfType<IronFiling>();
            return false;
        }
    }
}