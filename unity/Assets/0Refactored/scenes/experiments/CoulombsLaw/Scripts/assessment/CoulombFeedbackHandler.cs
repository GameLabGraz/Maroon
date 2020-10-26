using System;
using System.Linq;
using Antares.Evaluation;
using UnityEngine;

namespace Maroon.Assessment.Handler
{
    public class CoulombFeedbackHandler : AssessmentFeedbackHandler
    {
        [SerializeField] private GameObject chargePrefab;

        protected override AssessmentObject HandleObjectCreation(ManipulateObject manipulateObject)
        {
            AssessmentObject assessmentObject = null;

            var classType = manipulateObject.DataUpdates.First(updateData => updateData.PropertyName == "class");
            if(!Enum.TryParse(classType.Value.ToString(), true, out AssessmentClass assessmentClass)) return null;

            if (assessmentClass == AssessmentClass.Charge)
            {
                if (chargePrefab == null) return null;

                var position = (Vector3D)manipulateObject.DataUpdates.First(updateData => updateData.PropertyName == "position").Value;
                Debug.Log("Antares position " + position.ToVector3());

                assessmentObject = CoulombLogic.Instance
                    .CreateCharge(chargePrefab, Vector3.zero, 0, false, false)
                    .GetComponent<AssessmentObject>();
                
            }
            return assessmentObject;
        }
    }
}
