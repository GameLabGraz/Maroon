using UnityEngine;

namespace LimeSurveyData
{
    public class HuygensDataCollector : DataCollector
    {
        [SerializeField] private HuygensPrincipleData _experimentData;       
        
        private HuygensPrincipleMeasurements Measurements => (HuygensPrincipleMeasurements)_measurements;

        protected override void Start()
        {
            base.Start();
            _measurements = new HuygensPrincipleMeasurements();
            _experimentData.measurements.Add(Measurements);
        }
      
        public void OnSlitNumberChanged(int newValue)
        {
            Measurements.SlitNumberChanges.Add(new TimeMeasurementInt(newValue));
        }
        
        public void OnSlitWidthChanged(float newValue)
        {
            Measurements.SlitWidthChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnWaveAmplitudeChanged(float newValue)
        {
            Measurements.WaveAmplitudeChanges.Add(new TimeMeasurementFloat(newValue));
        }       
        
        public void OnWaveLengthChanged(float newValue)
        {
            Measurements.WaveLengthChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnWaveFrequencyChanged(float newValue)
        {
            Measurements.WaveFrequencyChanges.Add(new TimeMeasurementFloat(newValue));
        }
        
        public void OnPropagationModeChanged(int newValue)
        {
            Measurements.PropagationModeChanges.Add(new TimeMeasurementInt(newValue));
        }

        public void OnSlitPlateMoved(float newValue)
        {
            Measurements.PlateMovementChanges.Add(new TimeMeasurementFloat(newValue));
        }

        public void ConvertToJson()
        {
            Debug.Log(JsonUtility.ToJson(Measurements));
        }
    }
}
