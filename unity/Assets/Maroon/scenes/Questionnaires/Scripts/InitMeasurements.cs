
using UnityEngine;

namespace LimeSurveyData
{
    public class InitMeasurements : MonoBehaviour
    {
        [SerializeField] private LaboratoryData measurements;

        private void Awake()
        {
            measurements.ResponseID = -1;

            foreach (var experimentMeasurements in measurements.LaboratoryMeasurements)
            {
                experimentMeasurements.ExperimentMeasurements.RestData();
            }
        }
    }
}
