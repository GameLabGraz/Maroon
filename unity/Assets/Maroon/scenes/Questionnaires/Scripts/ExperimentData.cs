using System;
using System.Collections.Generic;
using UnityEngine;

namespace LimeSurveyData
{
    [Serializable] public abstract class MeasurementData : ScriptableObject
    {
        public abstract string ToJson();
        public abstract void RestData();
    }

    public abstract class ExperimentData<Measurements> : MeasurementData
    {
        public List<Measurements> measurements;
        public override string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(measurements);
        }

        public override void RestData()
        {
            measurements.Clear();
        }
    }

    [Serializable] public abstract class ExperimentMeasurements
    {
        [Header("Experiment Score - Coins")]
        [Tooltip("How many coins were collected.")]
        public int ExperimentScore = 0;

        [Header("Time Measurements")]
        [Tooltip("Total Time the user spends in the scene.")]
        public float TotalTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the experiment.")]
        public float ExperimentTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the quest manager.")]
        public float QuestManagerTime = 0f;
        [Tooltip("Time the user spends in the teleport zone of the whiteboard.")]
        public float WhiteboardTime = 0f;

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
}