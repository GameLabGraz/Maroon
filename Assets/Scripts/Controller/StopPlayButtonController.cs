using UnityEngine;
using VRTK;

public class StopPlayButtonController : VRTK_InteractableObject
{
    private SimulationController simController;

    protected override void Start()
    {
        base.Start();

        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

    }

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        Debug.Log("Stop Play Button pressed, will stop/play simulation");

        simController.SimulationRunning = !simController.SimulationRunning;
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
    }
}
