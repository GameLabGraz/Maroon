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

                assessmentObject = CoulombLogic.Instance
                    .CreateCharge(chargePrefab, Vector3.zero, 0, false, false)
                    .GetComponent<AssessmentObject>();
                
            }
            return assessmentObject;
        }

        protected override void HandleObjectDelete(AssessmentObject obj)
        {
            if (!obj) return;
            switch (obj.ClassType)
            {
                case AssessmentClass.Charge:
                    var chargeBehaviour = obj.GetComponent<CoulombChargeBehaviour>();
                    if(chargeBehaviour)
                        chargeBehaviour.MovementEndOutsideBoundaries();
                    var selectObj = obj.GetComponent<PC_SelectScript>();
                    if(selectObj)
                        selectObj.DeselectMe();
                    Destroy(obj.gameObject);
                    break;
                case AssessmentClass.Ruler:
                    var startPos = obj.GetWatchValue("start_position");
                    startPos?.SystemSetsQuantity(new Vector3(-1f, -1f, -1f));
                    var endPos = obj.GetWatchValue("end_position");
                    endPos?.SystemSetsQuantity(new Vector3(-1f, -1f, -1f));
                    break;
                case AssessmentClass.Voltmeter:
                    var startTerminal = obj.GetWatchValue("positive_terminal_position");
                    startTerminal?.SystemSetsQuantity(new Vector3(-1f, -1f, -1f));
                    var endTerminal = obj.GetWatchValue("negative_terminal_position");
                    endTerminal?.SystemSetsQuantity(new Vector3(-1f, -1f, -1f));
                    break;
            }
        }
    }
}
