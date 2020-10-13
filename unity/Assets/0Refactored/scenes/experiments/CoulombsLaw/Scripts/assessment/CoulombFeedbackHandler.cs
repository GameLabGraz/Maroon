using Antares.Evaluation;
using UnityEngine;

namespace Maroon.Assessment.Handler
{
    public class CoulombFeedbackHandler : AssessmentFeedbackHandler
    {
        [SerializeField] private CoulombLogic logic;
        [SerializeField] private GameObject chargePrefab;

        protected override AssessmentObject HandleObjectCreation(ManipulateObject manipulateObject)
        {
            AssessmentObject assessmentObject;
            if (logic == null || chargePrefab == null) return null;

            // ToDO:
            // if (manipulationObject.class == AssessmentClass.Charge)
            {
                assessmentObject = logic.CreateCharge(chargePrefab, Vector3.zero, 0, false)
                    .GetComponent<AssessmentObject>();
            }

            return assessmentObject;
        }
    }
}
