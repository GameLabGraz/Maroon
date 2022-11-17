using System.Collections.Generic;
using Maroon.UI.Charts;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class GraphRenderer : MonoBehaviour
    {
        [SerializeField] private MutableLineChart lineChart;
        [SerializeField] private float width = 1;
        [SerializeField] private float speed = 0.2f;
        [SerializeField] private bool animated = true;


        private float[] _data = new float[100];
        private List<float> _data2 = new List<float>();
        private float _y;

        private void Update()
        {
            if (!animated) return;
            _y += Time.deltaTime * speed;
            UpdateGraph();
        }

        public void UpdateGraph()
        {
            NoiseExperimentBase.Instance.GetNoise(ref _data, width, _y);
            lineChart.SetData(_data);

            NoiseExperimentBase.Instance.GetNoiseSizeDependent(ref _data2, width, _y);
            lineChart.SetData(_data2, 1);

            var threshold = NoiseExperimentBase.Instance.GetThreshold();
            lineChart.SetData(2, threshold, threshold);
        }
    }
}