using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class ObjectCorrectlyPlaced : IQuestCheck
    {
        public DragObject source;

        protected override void InitCheck()
        {
            source = FindObjectOfType<DragObject>();
        }

        protected override bool CheckCompliance()
        {
            //Debug.Log("checking.: " + _source.source_snapped);
            return source.source_snapped;
        }
    }
}