using GEAR.Localization;
using UnityEngine;

namespace Maroon.UI
{
    [RequireComponent(typeof(XCharts.LineChart))]
    public abstract class BaseLineChart : PausableObject, IResetObject
    {
        protected XCharts.LineChart Chart;

        public XCharts.Serie Serie0 => Chart.series.GetSerie(0);

        public string Title
        {
            get => Chart.title.text;
            set => Chart.title.text = value;
        }

        public string XAxisName
        {
            get => Chart.xAxis0.axisName.name;
            set => Chart.xAxis0.axisName.name = value;
        }

        public string YAxisName
        {
            get => Chart.yAxis0.axisName.name;
            set => Chart.yAxis0.axisName.name = value;
        }

        protected virtual void Awake()
        {
            Chart = GetComponent<XCharts.LineChart>();
        }

        protected virtual void Start()
        {
            Title = LanguageManager.Instance.GetString(Title);
            XAxisName = LanguageManager.Instance.GetString(XAxisName);
            YAxisName = LanguageManager.Instance.GetString(YAxisName);

            base.Start();
        }

        public virtual void ResetObject()
        {
            for (var i = 0; i < Chart.series.Count; i++)
                Chart.series.GetSerie(i).ClearData();
        }
    }
}
