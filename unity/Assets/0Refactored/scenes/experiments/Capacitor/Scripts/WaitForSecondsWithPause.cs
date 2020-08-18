using UnityEngine;

public class WaitForSecondsWithPause : CustomYieldInstruction
{
    private float waitTime;

    public override bool keepWaiting
    {
        get
        {
            if (waitTime >= 0)
            {
                if(SimulationController.Instance == null || SimulationController.Instance.SimulationRunning)
                    waitTime -= Time.deltaTime;

                return true;
            }

            return false;
        }
    }

    public WaitForSecondsWithPause(float seconds)
    {
        waitTime = seconds;
    }

    public WaitForSecondsWithPause(float seconds, SimulationController simController)
    {
        waitTime = seconds;
    }
}
