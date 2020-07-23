using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class IronFillingCheck : IQuestCheck
    {
        private scrIronFilings _ironFiling;

        protected override void InitCheck()
        {
            _ironFiling = FindObjectOfType<scrIronFilings>();
        }

        protected override bool CheckCompliance()
        {
            if (_ironFiling != null) return _ironFiling.gameObject.activeSelf;
            
            _ironFiling = FindObjectOfType<scrIronFilings>();
            return false;
        }
    }
}