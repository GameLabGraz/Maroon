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
}