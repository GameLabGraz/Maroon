using UnityEngine;

namespace LimeSurveyData
{

    public class TimeMeasurementArea : MonoBehaviour
    {
        [SerializeField] private DataCollector.MeasurementArea measurementArea;

        private DataCollector _dataCollector;

        private void Start()
        {
            _dataCollector = FindObjectOfType<DataCollector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player") || _dataCollector == null) return;

            Debug.Log($"{other.tag} entered time measurement area: {measurementArea}");

            _dataCollector.EnterMeasurementArea(measurementArea);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") || _dataCollector == null) return;

            Debug.Log($"{other.tag} left time measurement area: {measurementArea}");
            
            _dataCollector.LeaveMeasurementArea(measurementArea);
        }
    }
}