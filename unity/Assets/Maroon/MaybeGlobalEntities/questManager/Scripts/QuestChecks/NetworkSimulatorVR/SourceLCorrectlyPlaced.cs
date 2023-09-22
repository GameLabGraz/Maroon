using UnityEngine;
using Maroon.Experiments.NetworkSimulatorVR;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class SourceLCorrectlyPlaced : IQuestCheck
    {
        private DragObject flying_object;

        protected override void InitCheck()
        {
            var sourceLGameObject = GameObject.Find("SourceL");
            flying_object = sourceLGameObject.GetComponent<DragObject>();
          
        }

        protected override bool CheckCompliance()
        {

            return flying_object.source_snapped;
        }
    }
}