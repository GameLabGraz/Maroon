using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MSourceCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("SourceM");
            flying_object = sourceLGameObject.GetComponent<DragObjectMiddle>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.source_snapped;
        }
    }
}