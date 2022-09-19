using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimeSurveyData
{
    public class TimeIncluded<T>
    {
        private T _value;
        private float _time;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _time = Time.time;
            }
        }
        
        public TimeIncluded(T init)
        {
            Value = init;
        }
    }

    [Serializable]
    public class SurveySceneInfo
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
        public List<TimeIncluded<bool>> PressedStartSimulation = new List<TimeIncluded<bool>>();
        [Tooltip("How often the user reset the simulation.")]
        public List<float> PressedResetSimulation = new List<float>(); //float is here only the timestamp when the user pressed reset
        
        [Header("Customization Infos")] 
        [Tooltip("How often, to what and when the floor changed.")]
        public List<TimeIncluded<int>> FloorChanges = new List<TimeIncluded<int>>();
        [Tooltip("How often, to what and when the wall changed.")]
        public List<TimeIncluded<int>> WallChanges = new List<TimeIncluded<int>>();
        [Tooltip("How often, to what and when the deco visibility changed.")]
        public List<TimeIncluded<bool>> DecoChanges = new List<TimeIncluded<bool>>();
        [Tooltip("How often, to what and when the plant visibility changed.")]
        public List<TimeIncluded<bool>> PlantChanges = new List<TimeIncluded<bool>>();
        [Tooltip("How often, to what and when the pictures changed.")]
        public List<TimeIncluded<int>> PictureChanges = new List<TimeIncluded<int>>();
        [Tooltip("How often, to what and when the hand models changed.")]
        public List<TimeIncluded<int>> HandModelChanges = new List<TimeIncluded<int>>();
        [Tooltip("How often, to what and when the hand controller visibility changed.")]
        public List<TimeIncluded<bool>> HandControllerChanges = new List<TimeIncluded<bool>>();
    }

    [Serializable]
    public class HuygensSurveySceneInfo : SurveySceneInfo
    {
        [Header("Huygens Experiment Infos")] 
        [Tooltip("How often, to what and when the slit number changed.")]
        public List<TimeIncluded<int>> SlitNumberChanges = new List<TimeIncluded<int>>();
        [Tooltip("How often, to what and when the slit width changed.")]
        public List<TimeIncluded<float>> SlitWidthChanges = new List<TimeIncluded<float>>();
        [Tooltip("How often, to what and when the wave amplitude changed.")]
        public List<TimeIncluded<float>> WaveAmplitudeChanges = new List<TimeIncluded<float>>();
        [Tooltip("How often, to what and when the wave length changed.")]
        public List<TimeIncluded<float>> WaveLengthChanges = new List<TimeIncluded<float>>();
        [Tooltip("How often, to what and when the wave frequency changed.")]
        public List<TimeIncluded<float>> WaveFrequencyChanges = new List<TimeIncluded<float>>();
        [Tooltip("How often, to what and when the propagation mode changed.")]
        public List<TimeIncluded<int>> PropagationModeChanges = new List<TimeIncluded<int>>();
    }
}