using GEAR.Localization;
using UnityEngine;

namespace Maroon.UI.Charts
{
    public class LocalizedXChart : MonoBehaviour
    {
        private string _titleKey;
        private string _xAxisKey;
        private string _yAxisKey;

        protected XCharts.LineChart Chart;

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

        private void Start()
        {
            _titleKey = Title;
            _xAxisKey = XAxisName;
            _yAxisKey = YAxisName;

            Title = LanguageManager.Instance.GetString(_titleKey);
            XAxisName = LanguageManager.Instance.GetString(_xAxisKey);
            YAxisName = LanguageManager.Instance.GetString(_yAxisKey);

            LanguageManager.Instance.OnLanguageChanged.AddListener(language =>
            {
                Title = LanguageManager.Instance.GetString(_titleKey);
                XAxisName = LanguageManager.Instance.GetString(_xAxisKey);
                YAxisName = LanguageManager.Instance.GetString(_yAxisKey);
            });
        }
    }
}
