using UnityEngine;
using VRTK;

public class StopPlayButtonController : VRTK_InteractableObject
{
    [SerializeField]
    private TextMesh PlayPauseText;

    private SimulationController simController;

    private void Start()
    {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

    }

    protected override void Update()
    {
        base.Update();

        if (PlayPauseText != null)
            PlayPauseText.text = simController.SimulationRunning ? "PAUSE\nSimulation" : "PLAY\nSimulation";
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
