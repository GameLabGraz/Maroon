namespace Maroon.UI
{

    public class SimulationStartButton : SimulationButton
    {
        private void Start()
        {
            if (!SimulationController.Instance) return;

            gameObject.SetActive(!SimulationController.Instance.SimulationRunning);

            SimulationController.Instance.OnStart += (sender, args) => { gameObject.SetActive(false); };

            SimulationController.Instance.OnStop += (sender, args) => { gameObject.SetActive(true); };
        }
    }
}
