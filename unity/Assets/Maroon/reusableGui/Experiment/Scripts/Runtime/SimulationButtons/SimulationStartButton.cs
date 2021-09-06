namespace Maroon.UI
{

    public class SimulationStartButton : SimulationButton
    {
        private void Start()
        {
            if (!SimulationController.Instance) return;

            gameObject.SetActive(!SimulationController.Instance.SimulationRunning);

            SimulationController.Instance.OnStart.AddListener(() => { gameObject.SetActive(false); });

            SimulationController.Instance.OnStop.AddListener(() => { gameObject.SetActive(true); });
        }
    }
}
