using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MGatewayCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("GatewayM");
            flying_object = sourceLGameObject.GetComponent<DragObjectMiddle>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.gateway_snapped;
        }
    }
}