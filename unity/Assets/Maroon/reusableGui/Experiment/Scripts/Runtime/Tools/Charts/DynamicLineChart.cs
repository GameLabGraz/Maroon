using System;
using UnityEngine;

namespace Maroon.UI.Charts
{
    [Serializable] public class ValueGetterCallback : SerializableCallback<float> { };

    public class DynamicLineChart : BaseLineChart
    {
        [SerializeField] private ValueGetterCallback valueGetter;

        private int _time;
        private int _maxValueCount;

        protected override void Start()
        {
            base.Start();
            _maxValueCount = (int)Chart.xAxis0.max;
        }

        protected override void HandleUpdate() { }

        protected override void HandleFixedUpdate()
        {
            if (valueGetter == null) return;

            if (Chart.series.GetSerie(0).GetDataList().Count > _maxValueCount)
            {
                Chart.xAxis0.min = Chart.xAxis0.max;
                Chart.xAxis0.max += _maxValueCount;
                Serie0.ClearData();
            }

            Chart.AddData(0, _time++, valueGetter.Invoke());
        }

        public override void ResetObject()
        {
            _time = 0;
            Chart.xAxis0.min = 0;
            Chart.xAxis0.max = _maxValueCount;

            base.ResetObject();
        }
    }
}
