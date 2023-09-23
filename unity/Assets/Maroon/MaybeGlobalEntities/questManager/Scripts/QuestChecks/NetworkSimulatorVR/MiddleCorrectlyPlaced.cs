using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MiddleCorrectlyPlaced : IQuestCheck
    {
        private DragObject source;
        private DragObject destination;
        private DragObject gateway;

        private bool left_completed = false;

        protected override void InitCheck()
        {
            var sourceGameObject = GameObject.Find("SourceM");
            var destinationGameObject = GameObject.Find("DestinationM");
            var gatewayGameObject = GameObject.Find("GatewayM");

            source = sourceGameObject.GetComponent<DragObject>();
            destination = destinationGameObject.GetComponent<DragObject>();
            gateway = gatewayGameObject.GetComponent<DragObject>();

        }

        protected override bool CheckCompliance()
        {
            if ((source.source_snapped == true) &&
                (destination.destination_snapped == true) &&
                (gateway.gateway_snapped == true))
            {
                left_completed = true;
            }
            Debug.Log("middle: " + left_completed);
            return left_completed;
        }
    }
}