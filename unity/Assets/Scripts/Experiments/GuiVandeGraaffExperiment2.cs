using UnityEngine;

public class GuiVandeGraaffExperiment2 : MonoBehaviour {

	private VandeGraaffController vandeGraaffController;
	private GrounderController grounderController;
	private BalloonGlowController balloonGlowController;
	private bool glowEnabled = true;
	private GUIStyle textStyle;

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

	public void Update()
	{
        // NEW init, TODO 0 check l,r - maybe invalid if system null. dont do this every update
        var system = Valve.VR.OpenVR.System;
        var left = 0;
        var right = 0;
        if (system != null)
        {
            left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
            right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        }
        
        // check if [E] was pressed (Switch ON/OFF VdG)
        //if (Input.GetKeyDown (KeyCode.E)) 
        if (SteamVR_Controller.Input((int)left).GetPress(SteamVR_Controller.ButtonMask.Trigger) ||
         SteamVR_Controller.Input((int)right).GetPress(SteamVR_Controller.ButtonMask.Trigger))
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
        // VIVE -> ESC == MENU BUTTON top 
        //if (Input.GetKeyDown (KeyCode.Escape))  desktop
        if (SteamVR_Controller.Input((int)left).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) ||
         SteamVR_Controller.Input((int)right).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Application.LoadLevel("Laboratory");
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
		GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height / 16, 400f, 100f), string.Format ("You can turn the Van de Graaff Generator ON/OFF.\r\nExperiment and observe what happens.", this.vandeGraaffController.On ? "OFF" : "ON"), this.textStyle);
		// show voltage / charge of VdG
		GUI.Label (new Rect (Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format ("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", this.vandeGraaffController.GetVoltage(), this.vandeGraaffController.ChargeStrength));
		// show controls
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - Switch {0} Van de Graaff Generator\r\n[F] - {1} Electric Field\r\n[C] - {2} Charge", this.vandeGraaffController.On ? "OFF" : "ON", this.vandeGraaffController.FieldLinesEnabled ? "Hide" : "Show", this.glowEnabled ? "Hide" : "Show"));
	}
}
