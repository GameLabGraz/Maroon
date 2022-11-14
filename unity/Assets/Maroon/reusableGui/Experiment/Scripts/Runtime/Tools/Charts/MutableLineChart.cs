﻿using System.Collections.Generic;
using XCharts;

namespace Maroon.UI.Charts
{
    public class MutableLineChart : BaseLineChart
    {
        private int _time;
        private int _maxValueCount;

        protected override void Start()
        {
            base.Start();
            _maxValueCount = (int)Chart.xAxis0.max;
        }

        protected override void HandleUpdate()
        {
        }

        protected override void HandleFixedUpdate()
        {
        }

        public void SetData(IReadOnlyList<float> values, int series = 0)
        {
            var serie = Chart.series.GetSerie(series) ?? Chart.series.AddSerie(SerieType.Line, string.Empty);
            if (serie.data == null)
                serie.AddXYData(0, 0);

            if (serie.data.Count > values.Count)
                serie.data.RemoveRange(values.Count, serie.data.Count - values.Count);

            for (int i = 0; i < serie.data.Count; i++)
            {
                serie.data[i].data[0] = i / (values.Count - 1f);
                serie.data[i].data[1] = values[i];
            }

            for (int i = serie.data.Count; i < values.Count; i++)
            {
                serie.AddXYData(i / (values.Count - 1f), values[i]);
            }


            Chart.series.GetSerie(series).SetAllDirty();
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