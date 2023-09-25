using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class RightCorrectlyPlaced : IQuestCheck
    {
        private DragObjectRight source;
        private DragObjectRight destination;
        private DragObjectRight gateway;

        private bool right_completed = false;

        protected override void InitCheck()
        {
            var sourceGameObject = GameObject.Find("SourceR");
            var destinationGameObject = GameObject.Find("DestinationR");
            var gatewayGameObject = GameObject.Find("GatewayR");

            source = sourceGameObject.GetComponent<DragObjectRight>();
            destination = destinationGameObject.GetComponent<DragObjectRight>();
            gateway = gatewayGameObject.GetComponent<DragObjectRight>();

        }

        protected override bool CheckCompliance()
        {
            if ((source.source_snapped == true) &&
                (destination.destination_snapped == true) &&
                (gateway.gateway_snapped == true))
            {
                right_completed = true;
            }
            Debug.Log("left: " + right_completed);

            return right_completed;
        }
    }
}