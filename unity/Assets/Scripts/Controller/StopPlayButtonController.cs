using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class StopPlayButtonController : VRTK_InteractableObject
{
    [SerializeField]
    private Text PlayPauseText;

    protected override void Update()
    {
        base.Update();

        if (PlayPauseText != null)
            PlayPauseText.text = SimulationController.Instance.SimulationRunning ? "PAUSE\nSimulation" : "PLAY\nSimulation";
    }

    public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing(usingObject);

        Debug.Log("Stop Play Button pressed, will stop/play simulation");

        if(SimulationController.Instance.SimulationRunning)
            SimulationController.Instance.StopSimulation();
        else
            SimulationController.Instance.StartSimulation();
    }
}
