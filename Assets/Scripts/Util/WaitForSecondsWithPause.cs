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
                if(simController.SimulationRunning)
                {
                    waitTime -= Time.deltaTime;
                    Debug.Log(waitTime);
                }
                else
                {
                    Debug.Log("Pause");
                }

               

                return true;
            }            
            else
                return false;
        }
    }

    public WaitForSecondsWithPause(float seconds, SimulationController simController)
    {
        this.simController = simController;
        this.waitTime = seconds;
    }
}
