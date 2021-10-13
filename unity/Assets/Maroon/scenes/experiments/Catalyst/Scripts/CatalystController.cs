using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystController : MonoBehaviour
    {
        private void Start()
        {
            SimulationController.Instance.OnStart.AddListener(StartSimulation);
            SimulationController.Instance.OnStop.AddListener(StopSimulation);
        }

        private void OnDestroy()
        {
            SimulationController.Instance.OnStart.RemoveListener(StartSimulation);
            SimulationController.Instance.OnStop.RemoveListener(StopSimulation);
        }
        

        private void StartSimulation()
        {
            Debug.Log("Start catalyst simulation");
        }

        private void StopSimulation()
        {
            Debug.Log("Stop catalyst simulation");
        }
    }
}