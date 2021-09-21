namespace Maroon.UI
{

    public class SimulationStopButton : SimulationButton
    {
        private void Start()
        {
            if (!SimulationController.Instance) return;

            gameObject.SetActive(SimulationController.Instance.SimulationRunning);
            
            SimulationController.Instance.OnStart.AddListener(() => { gameObject.SetActive(true); });

            SimulationController.Instance.OnStop.AddListener(() => { gameObject.SetActive(false); });
        }
    }
}