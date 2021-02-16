using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(SubQuest))]
    public abstract class IQuestCheck : MonoBehaviour
    {
        private SubQuest _quest;

        private void Start()
        {
            InitCheck();

            _quest = GetComponent<SubQuest>();
            _quest.questCheck = CheckCompliance;
        }

        protected abstract void InitCheck();
        protected abstract bool CheckCompliance();
    }
}

