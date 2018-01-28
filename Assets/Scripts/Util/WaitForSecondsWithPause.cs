using UnityEngine;

public class WaitForSecondsWithPause : CustomYieldInstruction
{
    private SimulationController simController;
    private float waitTime;

    public override bool keepWaiting
    {
        get
        {
            if (waitTime >= 0)
            {
                if(simController == null || simController.SimulationRunning)
                    waitTime -= Time.deltaTime;

                return true;
            }

            return false;
        }
    }

    public WaitForSecondsWithPause(float seconds)
    {
        this.simController = GameObject.FindObjectOfType<SimulationController>();
        this.waitTime = seconds;
    }

    public WaitForSecondsWithPause(float seconds, SimulationController simController)
    {
        this.simController = simController;
        this.waitTime = seconds;
    }
}
