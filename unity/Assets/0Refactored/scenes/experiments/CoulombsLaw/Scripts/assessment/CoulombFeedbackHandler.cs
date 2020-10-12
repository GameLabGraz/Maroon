using Antares.Evaluation;
using UnityEngine;

namespace Maroon.Assessment.Handler
{
    public class CoulombFeedbackHandler : AssessmentFeedbackHandler
    {
        [SerializeField] private CoulombLogic logic;
        [SerializeField] private GameObject chargePrefab;

        protected override void HandleObjectCreation(ManipulateObject manipulateObject)
        {
            if (logic == null || chargePrefab == null) return;

            // DoTo: Check object type
            logic.CreateCharge(chargePrefab, Vector3.zero, 0, false);
        }
    }
}
