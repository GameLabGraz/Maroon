using System.Linq;

namespace QuestManager
{
    public class MainQuest : IQuest
    {
        private int _subQuestIndex = 0;

        protected override bool IsDone()
        {
            return SubQuests.All(subQuest => subQuest.IsFinished);
        }

        public void ActivateNextSubQuest()
        {
            if (_subQuestIndex >= SubQuests.Count)
                return;

            var activeSubQuest = SubQuests[_subQuestIndex++];

            activeSubQuest.IsActive = true;
            activeSubQuest.OnQuestFinished.AddListener(() =>
            {
                activeSubQuest.IsActive = false;
                ActivateNextSubQuest();
            });
        }
    }
}
