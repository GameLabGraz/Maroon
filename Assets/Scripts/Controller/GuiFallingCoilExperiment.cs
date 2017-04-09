using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiFallingCoilExperiment : MonoBehaviour
{

  private GUIStyle textStyle;

  private SimulationController simController;

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
  }

  public void OnGUI()
  {
    // show move Grounder message
    //GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height - (Screen.height / 2.5f), 400f, 100f), "You can turn the Van de Graaff Generator ON/OFF and move the Grounding Device Left/Right. Experiment and observe what happens.", this.textStyle);
    // show voltage / charge of VdG
    //GUI.Label(new Rect(Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", this.vandeGraaffController.GetVoltage(), this.vandeGraaffController.ChargeStrength));
    // show controls on top left corner
    GUI.Label(new Rect(10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - {0} Simulation \r\n[R] - Reset Simulation \r\n[S] - Step Simulation", simController.SimulationRunning ? "PAUSE" : "PLAY"));
  }
}