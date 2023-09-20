using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ObjectCorrectlyPlaced : IQuestCheck
    {
        //public DragObject source1;
        

        protected override void InitCheck()
        {
            //source = FindObjectOfType<SourceL>();
        }

        protected override bool CheckCompliance()
        {
            return true;
        }
    }
}