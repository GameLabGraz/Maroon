using System.Collections.Generic;
using System;
using UnityEngine;

namespace LimeSurveyData
{
    [CreateAssetMenu(fileName = "HuygensPrincipleData", menuName = "ExperimentData/HuygensPrincipleData")]
    public class HuygensPrincipleData : ExperimentData<HuygensPrincipleMeasurements>
    {

    }

    [Serializable] public class HuygensPrincipleMeasurements : ExperimentMeasurements
    {
        [Header("Huygens Experiment Infos")]
        [Tooltip("How often, to what and when the slit number changed.")]
        public List<TimeMeasurementInt> SlitNumberChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the slit width changed.")]
        public List<TimeMeasurementFloat> SlitWidthChanges = new List<TimeMeasurementFloat>();
        [Tooltip("How often, to what and when the wave amplitude changed.")]
        public List<TimeMeasurementFloat> WaveAmplitudeChanges = new List<TimeMeasurementFloat>();
        [Tooltip("How often, to what and when the wave length changed.")]
        public List<TimeMeasurementFloat> WaveLengthChanges = new List<TimeMeasurementFloat>();
        [Tooltip("How often, to what and when the wave frequency changed.")]
        public List<TimeMeasurementFloat> WaveFrequencyChanges = new List<TimeMeasurementFloat>();
        [Tooltip("How often, to what and when the propagation mode changed.")]
        public List<TimeMeasurementInt> PropagationModeChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the plate movement changed.")]
        public List<TimeMeasurementFloat> PlateMovementChanges = new List<TimeMeasurementFloat>();
    }
}
