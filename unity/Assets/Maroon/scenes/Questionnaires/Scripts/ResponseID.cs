using GameLabGraz.LimeSurvey;
using UnityEngine;

namespace LimeSurveyData
{
    public class ResponseID : MonoBehaviour
    {
        [SerializeField] private LimeSurveyView limeSurveyView;
        [SerializeField] private LaboratoryData measurements;

        private void Awake()
        {
            limeSurveyView.ResponseID = measurements.ResponseID;
            limeSurveyView.OnSubmissionFinished.AddListener(responseID =>
            {
                measurements.ResponseID = responseID;
            });
        }

    }
}
