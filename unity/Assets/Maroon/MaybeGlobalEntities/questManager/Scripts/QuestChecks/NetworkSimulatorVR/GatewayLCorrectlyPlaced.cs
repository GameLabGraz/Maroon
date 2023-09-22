using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class GatewayLCorrectlyPlaced : IQuestCheck
    {
        private DragObject flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("GatewayL");
            flying_object = sourceLGameObject.GetComponent<DragObject>();

        }

        protected override bool CheckCompliance()
        {

            return flying_object.gateway_snapped;
        }
    }
}