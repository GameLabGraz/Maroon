using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiFallingCoilExperiment : MonoBehaviour
{

  private GUIStyle textStyle;

  private SimulationController simController;

  [SerializeField]
  private IronFiling ironFiling;

  void Start ()
  {
        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

        // define GUI style
        this.textStyle = new GUIStyle("label");
        this.textStyle.alignment = TextAnchor.MiddleCenter;
  }

  // Update is called once per frame
  void Update()
  {
    // check if [ESC] was pressed
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      SceneManager.LoadScene("Laboratory");
    }

    // check if [E] was pressed (PLAY/PAUSE Simulation)
    if (Input.GetKeyDown(KeyCode.E))
    {
      if (simController.SimulationRunning)
        simController.StopSimulation();
      else
        simController.StartSimulation();
    }

    // check if [R] was pressed (Reset Simulation)
    if (Input.GetKeyDown(KeyCode.R))
    {
      simController.ResetSimulation();
    }

    // check if [S] was pressed (Step Simulation)
    if (Input.GetKeyDown(KeyCode.S))
    {
      simController.SimulateStep();
    }

    // check if [I] was pressed (Iron Filings)
    if(Input.GetKeyDown(KeyCode.I))
    {
      if(ironFiling)
        ironFiling.generateFieldImage();
    }
  }

  public void OnGUI()
  {
    // show controls on top left corner
    GUI.Label(new Rect(10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n" +
        "[E] - {0} Simulation \r\n" +
        "[R] - Reset Simulation \r\n" +
        "[S] - Step Simulation", simController.SimulationRunning ? "PAUSE" : "PLAY") + "\r\n" +
        "[I] - Iron Filings");
  }
}