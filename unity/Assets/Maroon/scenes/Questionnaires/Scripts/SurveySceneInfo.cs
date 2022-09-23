using System;
using System.Collections.Generic;
using UnityEngine;

namespace LimeSurveyData
{
    public class TimeMeasurement<T>
    {
        [SerializeField] private T _value;
        [SerializeField] private float _time;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _time = Time.time;
            }
        }
        public TimeMeasurement(T init)
        {
            Value = init;
        }
    }

    [Serializable] public class TimeMeasurementBool : TimeMeasurement<bool>
    {
        public TimeMeasurementBool(bool init) : base(init) { }
    }
    [Serializable] public class TimeMeasurementInt : TimeMeasurement<int>
    {
        public TimeMeasurementInt(int init) : base(init) { }
    }
    [Serializable] public class TimeMeasurementFloat : TimeMeasurement<float>
    {
        public TimeMeasurementFloat(float init) : base(init) { }
    }


    [Serializable]
    public class ExperimentMeasurements
    {
        public string SceneName;
        
        [Header("Time Measurements")]
        [Tooltip("Total Time the user spends in the scene.")]
        public float TotalTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the experiment.")]
        public float ExperimentTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the quest manager.")]
        public float QuestManagerTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the whiteboard.")]
        public float WhiteboardTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the exit door.")]
        public float ExitDoorTime = 0f;

        [Header("General Button Counts")] 
        [Tooltip("How often the user started (resp. stopped) the simulation.")]
        public List<TimeMeasurementBool> PressedStartSimulation = new List<TimeMeasurementBool>();
        [Tooltip("How often the user reset the simulation.")]
        public List<float> PressedResetSimulation = new List<float>(); //float is here only the timestamp when the user pressed reset
        
        [Header("Customization Infos")] 
        [Tooltip("How often, to what and when the floor changed.")]
        public List<TimeMeasurementInt> FloorChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the wall changed.")]
        public List<TimeMeasurementInt> WallChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the deco visibility changed.")]
        public List<TimeMeasurementBool> DecoChanges = new List<TimeMeasurementBool>();
        [Tooltip("How often, to what and when the plant visibility changed.")]
        public List<TimeMeasurementBool> PlantChanges = new List<TimeMeasurementBool>();
        [Tooltip("How often, to what and when the pictures changed.")]
        public List<TimeMeasurementInt> PictureChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the hand models changed.")]
        public List<TimeMeasurementInt> HandModelChanges = new List<TimeMeasurementInt>();
        [Tooltip("How often, to what and when the hand controller visibility changed.")]
        public List<TimeMeasurementBool> HandControllerChanges = new List<TimeMeasurementBool>();
    }

    [Serializable]
    public class HuygensPrincipleMeasurements : ExperimentMeasurements
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
    }
}