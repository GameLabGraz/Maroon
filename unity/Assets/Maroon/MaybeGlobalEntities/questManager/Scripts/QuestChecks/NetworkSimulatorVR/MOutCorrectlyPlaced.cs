using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class MOutCorrectlyPlaced : IQuestCheck
    {
        private DragObjectMiddle outgoing;

        protected override void InitCheck()
        {
            var outgoingGameObject = GameObject.Find("Outgoing");
            outgoing = outgoingGameObject.GetComponent<DragObjectMiddle>();


        }

        protected override bool CheckCompliance()
        {
            return outgoing.position_snapped;
        }
    }
}