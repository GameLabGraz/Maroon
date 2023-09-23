using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MDestinationCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("DestinationM");
            flying_object = sourceLGameObject.GetComponent<DragObjectMiddle>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.destination_snapped;
        }
    }
}