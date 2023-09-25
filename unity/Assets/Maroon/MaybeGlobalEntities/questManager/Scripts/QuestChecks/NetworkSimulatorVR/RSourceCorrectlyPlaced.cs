using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class RSourceCorrectlyPlaced : IQuestCheck
    {
        private DragObjectRight flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("SourceR");
            flying_object = sourceLGameObject.GetComponent<DragObjectRight>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.source_snapped;
        }
    }
}