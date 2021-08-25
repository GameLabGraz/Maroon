using UnityEngine;

namespace Maroon.UI
{
    public class SimulationButton : MonoBehaviour
    {
        public void StartSimulation()
        {
            if (!SimulationController.Instance) return;

            SimulationController.Instance.StartSimulation();
        }

        public void StopSimulation()
        {
            if (!SimulationController.Instance) return;

            SimulationController.Instance.StopSimulation();
        }

        public void SimulateStep()
        {
            if (!SimulationController.Instance) return;

            SimulationController.Instance.SimulateStep();
        }

        public void ResetSimulation()
        {
            if (!SimulationController.Instance) return;

            SimulationController.Instance.ResetSimulation();
        }
    }
}
