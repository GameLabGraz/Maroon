using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MGatewayCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle outgoing;
        private DragObjectMiddle incoming;

        protected override void InitCheck()
        {
            var outgoingGameObject = GameObject.Find("Outgoing");
            outgoing = outgoingGameObject.GetComponent<DragObjectMiddle>();

            var incomingGameObject = GameObject.Find("Incoming");
            incoming = incomingGameObject.GetComponent<DragObjectMiddle>();

        }

        protected override bool CheckCompliance()
        {
            bool ret_val = false;

            if ((outgoing.position_snapped == true) && ( incoming.position_snapped == true))
            {
                ret_val = true;
            }

            return ret_val;
        }
    }
}