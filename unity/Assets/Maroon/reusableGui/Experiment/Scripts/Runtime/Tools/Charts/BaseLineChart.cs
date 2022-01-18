using Maroon.Physics;
using UnityEngine;

namespace Maroon.UI.Charts
{
    [RequireComponent(typeof(XCharts.LineChart))]
    public abstract class BaseLineChart : PausableObject, IResetObject
    {

        protected XCharts.LineChart Chart;

        public XCharts.Serie Serie0 => Chart.series.GetSerie(0);

        protected virtual void Awake()
        {
            Chart = GetComponent<XCharts.LineChart>();
        }

        protected virtual void Start()
        {
            base.Start();
        }

        public virtual void ResetObject()
        {
            for (var i = 0; i < Chart.series.Count; i++)
                Chart.series.GetSerie(i).ClearData();
        }
    }
}
