using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GuiVandeGraaffExperiment1 : MonoBehaviour {

	private VandeGraaffController vandeGraaffController;
	private GrounderController grounderController;
	private PaperStripesController paperStripesController;
	private bool glowEnabled;
	private GUIStyle textStyle;
	
	public void Start () {
		// find Van de Graaff Generator object in the scene
		GameObject vandeGraaff = GameObject.FindGameObjectWithTag ("VandeGraaff");
		if (null != vandeGraaff) 
		{
			this.vandeGraaffController = vandeGraaff.GetComponent<VandeGraaffController>();
		}
		// find Grounder object in the scene
		GameObject grounder = GameObject.FindGameObjectWithTag ("Grounder");
		if (null != grounder) {
			this.grounderController = grounder.GetComponent<GrounderController>();
		}
		// find Paper Stripes object in the scene
		GameObject paperStripes = GameObject.FindGameObjectWithTag ("PaperStripes");
		if (null != paperStripes) {
			this.paperStripesController = paperStripes.GetComponent<PaperStripesController>();
		}
		this.EnableGlow (this.glowEnabled);

		// define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	public void Update()
	{
		// check if [E] was pressed (Switch ON/OFF VdG)
		if (Input.GetKeyDown (KeyCode.E)) 
		{
			this.vandeGraaffController.Switch();
		}

		// check if [C] was pressed (Show/Hide Charge Glow)
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			this.glowEnabled = !this.glowEnabled;
			this.EnableGlow(this.glowEnabled);
		}

		// check if [F] was pressed (Show/Hide Field Lines)
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			this.vandeGraaffController.FieldLinesEnabled = !this.vandeGraaffController.FieldLinesEnabled;
		}

		// check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
      SceneManager.LoadScene("Laboratory");
		}
	}

	private void EnableGlow(bool enable)
	{
		this.vandeGraaffController.GlowEnabled = enable;
		this.grounderController.GlowEnabled = enable;
		this.paperStripesController.GlowEnabled = enable;
	}

	// OnGUI is called once per frame
	public void OnGUI()
	{
		// show move Grounder message
		GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height - (Screen.height / 2.5f), 400f, 100f), "You can turn the Van de Graaff Generator ON/OFF and move the Grounding Device Left/Right. Experiment and observe what happens.", this.textStyle);
		// show voltage / charge of VdG
		GUI.Label (new Rect (Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format ("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", this.vandeGraaffController.GetVoltage(), this.vandeGraaffController.ChargeStrength));
		// show controls on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - Switch {0} Van de Graaff Generator\r\n[<-] or [A] - Move Left\r\n[->] or [D] - Move Right\r\n[F] - {1} Electric Field\r\n[C] - {2} Charge", this.vandeGraaffController.On ? "OFF" : "ON", this.vandeGraaffController.FieldLinesEnabled ? "Hide" : "Show", this.glowEnabled ? "Hide" : "Show"));
	}
}
