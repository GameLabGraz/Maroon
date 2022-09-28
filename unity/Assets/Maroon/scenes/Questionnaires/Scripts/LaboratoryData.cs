using System;
using System.Collections.Generic;
using UnityEngine;

namespace LimeSurveyData
{
    [Serializable]
    public class MeasurementDataEntry
    {
        public string ExperimentName;
        public MeasurementData ExperimentMeasurements;
    }

    [CreateAssetMenu(fileName = "LaboratoryData", menuName = "ExperimentData/LaboratoryData")]
    public class LaboratoryData : ScriptableObject
    {
        public int ResponseID;
        public List<MeasurementDataEntry> LaboratoryMeasurements;
    }
}