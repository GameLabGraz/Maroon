using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuiFallingCoilExperiment : MonoBehaviour
{

  private GUIStyle textStyle;
  public Text text_info;


    void Start ()
  {
    // define GUI style
    this.textStyle = new GUIStyle("label");
    this.textStyle.alignment = TextAnchor.MiddleCenter;
  }

  // Update is called once per frame
  void Update()
  {
    // check if [Space] was pressed
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SceneManager.LoadScene("Laboratory");
    }

    // check if [E] was pressed (PLAY/PAUSE Simulation)
    if (Input.GetKeyDown(KeyCode.E))
    {
      if (SimController.Instance.SimulationRunning)
        SimController.Instance.StopSimulation();
      else
        SimController.Instance.StartSimulation();
    }

    // check if [R] was pressed (Reset Simulation)
    if (Input.GetKeyDown(KeyCode.R))
    {
      SimController.Instance.ResetSimulation();
    }

    // check if [S] was pressed (Step Simulation)
    if (Input.GetKeyDown(KeyCode.S))
    {
      SimController.Instance.SimulateStep();
    }

        string dialogue;
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "FaradaysLaw")
           dialogue = GamificationManager.instance.l_manager.GetString("Faraday GUI");
        else
            dialogue = GamificationManager.instance.l_manager.GetString("Falling GUI");
        dialogue = dialogue.Replace("NEWLINE ", "\n");
        text_info.text = dialogue;

    }

    public void OnGUI()
  {
    // show move Grounder message
    //GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height - (Screen.height / 2.5f), 400f, 100f), "You can turn the Van de Graaff Generator ON/OFF and move the Grounding Device Left/Right. Experiment and observe what happens.", this.textStyle);
    // show voltage / charge of VdG
    //GUI.Label(new Rect(Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", this.vandeGraaffController.GetVoltage(), this.vandeGraaffController.ChargeStrength));
    // show controls on top left corner
   // GUI.Label(new Rect(10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - {0} Simulation \r\n[R] - Reset Simulation \r\n[S] - Step Simulation", SimController.Instance.SimulationRunning ? "PAUSE" : "PLAY"));
  }
}