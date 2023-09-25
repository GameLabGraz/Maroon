using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class RDestinationCorrectlyPlaced : IQuestCheck
    {
        private DragObjectRight flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("DestinationR");
            flying_object = sourceLGameObject.GetComponent<DragObjectRight>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.destination_snapped;
        }
    }
}