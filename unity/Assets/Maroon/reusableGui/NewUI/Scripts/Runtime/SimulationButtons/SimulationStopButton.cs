namespace Maroon.UI
{

    public class SimulationStopButton : SimulationButton
    {
        private void Start()
        {
            if (!SimulationController.Instance) return;

            gameObject.SetActive(SimulationController.Instance.SimulationRunning);
            
            SimulationController.Instance.OnStart += (sender, args) => { gameObject.SetActive(true); };

            SimulationController.Instance.OnStop += (sender, args) => { gameObject.SetActive(false); };
        }
    }
}