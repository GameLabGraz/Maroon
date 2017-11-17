using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GuiVandeGraaffExperiment2 : MonoBehaviour {

	private VandeGraaffController vandeGraaffController;
	private GrounderController grounderController;
	private BalloonGlowController balloonGlowController;
	private bool glowEnabled;
	private GUIStyle textStyle;
    public string gname;

    public Text text_voltage;
    public Text text_charge;
    public Text text_info1;


    public void Start () {
		// find Van der Graaf Generator object in the scene
		GameObject vandeGraaff = GameObject.FindGameObjectWithTag ("VandeGraaff");
		if (null != vandeGraaff) {
			this.vandeGraaffController = vandeGraaff.GetComponent<VandeGraaffController>();
		}
		// find Grounder object in the scene
		GameObject grounder = GameObject.FindGameObjectWithTag ("Grounder");
		if (null != grounder) {
			this.grounderController = grounder.GetComponent<GrounderController>();
		}
		// find Balloon object in the scene
		GameObject balloon = GameObject.FindGameObjectWithTag ("Balloon");
		if (null != balloon) {
			this.balloonGlowController = balloon.GetComponent<BalloonGlowController>();
		}
		this.EnableGlow (this.glowEnabled);

		// define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
    }


    //New mechanic, things are activated by clicking on them
    private void OnMouseDown()
    {
        if (gname == "generator")
        {
            this.vandeGraaffController.Switch();
        }
        else if (gname == "line")
        {

            this.vandeGraaffController.FieldLinesEnabled = !this.vandeGraaffController.FieldLinesEnabled;
        }
        else if (gname == "charge")
        {
            this.glowEnabled = !this.glowEnabled;
            this.EnableGlow(this.glowEnabled);
        }
    }

    //Old mechanic
    public void Update()
	{/*
		// check if [E] was pressed (Switch ON/OFF VdG)
		if (Input.GetKeyDown (KeyCode.E)) 
		{
			//this.vandeGraaffController.Switch();
		}

		// check if [C] was pressed (Show/Hide Charge Glow)
		if (Input.GetKeyDown (KeyCode.C)) 
		{
		//	this.glowEnabled = !this.glowEnabled;
			this.EnableGlow(this.glowEnabled);
		}

		// check if [F] was pressed (Show/Hide Field Lines)
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			this.vandeGraaffController.FieldLinesEnabled = !this.vandeGraaffController.FieldLinesEnabled;
		}
        */

        string dialogue;
        dialogue = GamificationManager.instance.l_manager.GetString("Voltage GUI") + this.vandeGraaffController.GetVoltage();
        dialogue = dialogue.Replace("NEWLINE ", "\n");
        text_voltage.text = dialogue;
        text_charge.text = GamificationManager.instance.l_manager.GetString("Charge GUI") + this.vandeGraaffController.ChargeStrength;
        dialogue = GamificationManager.instance.l_manager.GetString("Info 1 Vandegraaf 1");
        dialogue = dialogue.Replace("NEWLINE ", "\n");
        text_info1.text = dialogue;
        // check if [ESC] was pressed
        if (Input.GetKeyDown (KeyCode.Space)) 
		{
      SceneManager.LoadScene("Laboratory");
		}
	}

	private void EnableGlow(bool enable)
	{
		this.vandeGraaffController.GlowEnabled = enable;
		this.grounderController.GlowEnabled = enable;
		this.balloonGlowController.GlowEnabled = enable;
	}
	
	// OnGUI is called once per frame
	public void OnGUI()
	{
		// show info message
	//	GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height / 16, 400f, 100f), string.Format ("You can turn the Van de Graaff Generator ON/OFF.\r\nExperiment and observe what happens.", this.vandeGraaffController.On ? "OFF" : "ON"), this.textStyle);
		// show voltage / charge of VdG
		//GUI.Label (new Rect (Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format ("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", this.vandeGraaffController.GetVoltage(), this.vandeGraaffController.ChargeStrength));
		// show controls
		//GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - Switch {0} Van de Graaff Generator\r\n[F] - {1} Electric Field\r\n[C] - {2} Charge", this.vandeGraaffController.On ? "OFF" : "ON", this.vandeGraaffController.FieldLinesEnabled ? "Hide" : "Show", this.glowEnabled ? "Hide" : "Show"));
	}
}
