using System;
using System.Collections.Generic;

namespace Maroon.UI.Charts
{
    public class SimpleLineChart : BaseLineChart
    {
        private List<XCharts.SerieData> _values;

        protected override void Start()
        {
            base.Start();
            _values = Serie0.data;
        }

        public void AddData(float yValue)
        {
            Chart.AddData(0, yValue);
        }

        public void AddData(List<float> yValues)
        {
            Chart.AddData(0, yValues);
        }

        public void AddData(float xValue, float yValue)
        {
            Chart.AddData(0, xValue, yValue);
        }

        public void AddData(List<Tuple<float, float>> values)
        {
            foreach (var value in values)
                Chart.AddData(0, value.Item1, value.Item2);
        }

        protected override void HandleUpdate() { }

        protected override void HandleFixedUpdate() { }

        public override void ResetObject()
        {
            base.ResetObject();
            foreach (var value in _values)
                Serie0.AddData(value.data);
        }
    }
}
