using UnityEngine;

public class KeyboardControl : MonoBehaviour // former GuiFallingCoilExperiment
{
  private GUIStyle textStyle;

  [SerializeField]
  private scrIronFilings _ironFiling;

  private void Start()
  {
      textStyle = new GUIStyle("label") {alignment = TextAnchor.MiddleCenter};
  }

  private void Update()
  {
    // check if [E] was pressed (PLAY/PAUSE Simulation)
    if (Input.GetKeyDown(KeyCode.E))
    {
      if (SimulationController.Instance.SimulationRunning)
          SimulationController.Instance.StopSimulation();
      else
          SimulationController.Instance.StartSimulation();
    }

    // check if [R] was pressed (Reset Simulation)
    if (Input.GetKeyDown(KeyCode.R))
    {
        SimulationController.Instance.ResetSimulation();
    }

    // check if [S] was pressed (Step Simulation)
    if (Input.GetKeyDown(KeyCode.S))
    {
        SimulationController.Instance.SimulateStep();
    }

    // check if [I] was pressed (Iron Filings)
    if(Input.GetKeyDown(KeyCode.I))
    {
      if(_ironFiling)
        _ironFiling.generateFieldImage();
    }
  }

  public void OnGUI()
  {
    // show controls on top left corner
    GUI.Label(new Rect(10f, 10f, 300f, 200f), string.Format(
        "[E] - {0} Simulation \r\n" +
        "[R] - Reset Simulation \r\n" +
        "[S] - Step Simulation", SimulationController.Instance.SimulationRunning ? "PAUSE" : "PLAY") + "\r\n" +
        "[I] - Iron Filings");
  }
}