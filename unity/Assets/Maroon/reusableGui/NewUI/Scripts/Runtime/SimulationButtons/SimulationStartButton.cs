namespace Maroon.UI
{

    public class SimulationStartButton : SimulationButton
    {
        private void Start()
        {
            if (!SimulationController.Instance) return;

            SimulationController.Instance.OnStart += (sender, args) => { gameObject.SetActive(false); };

            SimulationController.Instance.OnStop += (sender, args) => { gameObject.SetActive(true); };
        }
    }
}
