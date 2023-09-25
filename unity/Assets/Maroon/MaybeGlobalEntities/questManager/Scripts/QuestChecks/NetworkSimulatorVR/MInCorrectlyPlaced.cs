using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MInCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle incoming;

        protected override void InitCheck()
        {
            var incomingGameObject = GameObject.Find("Incoming");
            incoming = incomingGameObject.GetComponent<DragObjectMiddle>();


        }

        protected override bool CheckCompliance()
        {
            return incoming.position_snapped;
        }
    }
}