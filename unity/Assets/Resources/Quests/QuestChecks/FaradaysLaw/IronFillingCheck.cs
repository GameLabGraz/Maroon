using System;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class IronFillingCheck : QuestCheck
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