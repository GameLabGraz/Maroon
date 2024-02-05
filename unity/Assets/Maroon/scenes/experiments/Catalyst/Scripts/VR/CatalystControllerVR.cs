using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;
using XCharts;

namespace Maroon.Chemistry.Catalyst.VR
{
    public class CatalystControllerVR : CatalystController
    {
        [Header("VR Objects")]
        [SerializeField] protected VRLinearDrive temperatureViewVr;
        [SerializeField] protected VRLinearDrive partialPressureViewVr;

        [SerializeField] protected List<LineChart> lineChartsVRBox;
        [SerializeField] protected CatalystVrControlPanel controlPanel;
        
        protected override void Start()
        {
            base.Start();

            isVrVersion = true;

            foreach (var chart in lineChartsVRBox)
                chart.gameObject.SetActive(false);
            
            catalystReactor.OnReactorFilled.AddListener(() =>
            {
                if (controlPanel)
                    controlPanel.Setup(Mathf.Min(MaxXCoord - MinXCoord, MaxZCoord - MinZCoord), _doStepWiseSimulation);
            });
        }

        public override void ResetObject()
        {
            base.ResetObject();

            foreach (var chart in lineChartsVRBox)
            {
                chart.gameObject.SetActive(false);
            }

            if (controlPanel)
                controlPanel.ResetToInitialPosition();
        }

        protected override void SetSimulationParametersMinMax(CatalystVariation variation)
        {
            base.SetSimulationParametersMinMax(variation);

            temperatureViewVr.SetMinMax(temperature.minValue, temperature.maxValue);
            partialPressureViewVr.SetMinMax(partialPressure.minValue, partialPressure.maxValue);
            temperatureViewVr.ForceToValue(temperature.minValue);
            partialPressureViewVr.ForceToValue(partialPressure.minValue);
        }

        protected override void DrawSimulationGraphs()
        {
            if (_doInteractiveSimulation)
            {
                var lineChartVRBox = lineChartsVRBox[(int)ExperimentVariation];
                lineChartVRBox.gameObject.SetActive(true);
                lineChartVRBox.RefreshChart();
            }
            else
            {
                var lineChart = lineCharts[(int)ExperimentVariation];
                lineChart.gameObject.SetActive(true);
                lineChart.series.RemoveAll();
                StartCoroutine(CoDrawSimulationGraphs(lineChart, _graphSeriesList[(int)ExperimentVariation], 1.6f));
                progressChart.gameObject.SetActive(true);
                StartCoroutine(CoDrawProgressGraph());
            }
        }
    }
}
