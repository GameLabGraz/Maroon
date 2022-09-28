using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLabGraz.LimeSurvey;
using GameLabGraz.LimeSurvey.Data;
using GEAR.Gadgets.Coroutine;
using UnityEngine;
using UnityEngine.Events;

namespace LimeSurveyData
{
    public class MeasurementUploader : MonoBehaviour
    {
        [SerializeField] private LaboratoryData measurements;
        
        public UnityEvent OnSubmission;
        
        private void Start()
        {
            Debug.Log("MeasurementUploader: "+GetJsonUploadData());
            StartCoroutine(SubmitMeasurements());
        }

        private string GetJsonUploadData()
        {
            return measurements.LaboratoryMeasurements.Aggregate(
                "{", (current, measurement) => current + $"{measurement.ExperimentName}:{measurement.ExperimentMeasurements.ToJson()}") + "}";
        }

        private IEnumerator SubmitMeasurements()
        {
            var cd = new CoroutineWithData(this, LimeSurveyManager.Instance.GetQuestionList());
            yield return cd.Coroutine;

            var questions = (List<Question>)cd.Result;

            questions[0].Answer = GetJsonUploadData();

            cd = new CoroutineWithData(this, LimeSurveyManager.Instance.UploadQuestionResponses(questions, measurements.ResponseID));
            yield return cd.Coroutine;

            if ((int)cd.Result == -1)
            {
                Debug.LogError("MeasurementUploader::SubmitResponses: Unable to submit responses.");
            }
            else
            {
                OnSubmission?.Invoke();
            }
        }
    }
}
