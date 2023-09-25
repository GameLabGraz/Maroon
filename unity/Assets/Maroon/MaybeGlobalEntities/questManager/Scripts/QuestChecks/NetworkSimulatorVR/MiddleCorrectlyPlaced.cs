using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MiddleCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle source;
        private DragObjectMiddle destination;
        private DragObjectMiddle outgoing;
        private DragObjectMiddle incoming;

        private bool left_completed = false;

        protected override void InitCheck()
        {
            var sourceGameObject = GameObject.Find("SourceM");
            var destinationGameObject = GameObject.Find("DestinationM");
            var outgoingGameObject = GameObject.Find("Outgoing");
            var incomingGameObject = GameObject.Find("Incoming");

            source = sourceGameObject.GetComponent<DragObjectMiddle>();
            destination = destinationGameObject.GetComponent<DragObjectMiddle>();
            outgoing = outgoingGameObject.GetComponent<DragObjectMiddle>();
            incoming = incomingGameObject.GetComponent<DragObjectMiddle>();

        }

        protected override bool CheckCompliance()
        {
            if ((source.source_snapped == true) &&
                (destination.destination_snapped == true) &&
                (outgoing.position_snapped == true) && 
                ( incoming.position_snapped == true))
            {
                left_completed = true;
            }
            Debug.Log("middle: " + left_completed);
            return left_completed;
        }
    }
}