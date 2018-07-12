using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuiVandeGraaffExperiment2 : MonoBehaviour {

	[HideInInspector] public VandeGraaffController vandeGraaffController;
    [HideInInspector] public GrounderController grounderController;
    [HideInInspector] public BalloonGlowController balloonGlowController;
    [HideInInspector] public bool glowEnabled;
	private GUIStyle textStyle;
    public Text voltageText, chargeText;
    public bool onGuiEnabled = false;


	public void Start () {
		// find Van der Graaf Generator object in the scene
		GameObject vandeGraaff = GameObject.FindGameObjectWithTag ("VandeGraaff");
		if (null != vandeGraaff) {
			vandeGraaffController = vandeGraaff.GetComponent<VandeGraaffController>();
		}
		// find Grounder object in the scene
		GameObject grounder = GameObject.FindGameObjectWithTag ("Grounder");
		if (null != grounder) {
			grounderController = grounder.GetComponent<GrounderController>();
		}
		// find Balloon object in the scene
		GameObject balloon = GameObject.FindGameObjectWithTag ("Balloon");
		if (null != balloon) {
			balloonGlowController = balloon.GetComponent<BalloonGlowController>();
		}
		EnableGlow (glowEnabled);

		// define GUI style
		textStyle = new GUIStyle("label");
		textStyle.alignment = TextAnchor.MiddleCenter;
	}

	public void Update()
	{
	    voltageText.text = "Voltage: " + vandeGraaffController.GetVoltage().ToString("#,###,###") + " V";
	    chargeText.text = "Charge: " + vandeGraaffController.ChargeStrength + " C";

        // check if [E] was pressed (Switch ON/OFF VdG)
        if (Input.GetKeyDown (KeyCode.E)) 
		{
			vandeGraaffController.Switch();
		}

		// check if [C] was pressed (Show/Hide Charge Glow)
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			glowEnabled = !glowEnabled;
			EnableGlow(glowEnabled);
		}

		// check if [F] was pressed (Show/Hide Field Lines)
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			vandeGraaffController.FieldLinesEnabled = !vandeGraaffController.FieldLinesEnabled;
		}

		// check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape))
        {
            StartCoroutine(VRSceneLoader.loadScene("Laboratory"));
        }
	}

	public void EnableGlow(bool enable)
	{
		vandeGraaffController.GlowEnabled = enable;
		grounderController.GlowEnabled = enable;
		balloonGlowController.GlowEnabled = enable;
	}
	
	// OnGUI is called once per frame
	public void OnGUI()
	{
	    if (!onGuiEnabled)
	        return;
		// show info message
		GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height / 16 * 8, 400f, 100f), string.Format ("You can turn the Van de Graaff Generator ON/OFF.\r\nExperiment and observe what happens.", vandeGraaffController.On ? "OFF" : "ON"), textStyle);
        // show voltage / charge of VdG
        GUI.Label(new Rect(Screen.width - 170f, Screen.height - 50f, 170f, 50f), string.Format("Voltage: {0,15:N0} V\r\nCharge: {1,16} C", vandeGraaffController.GetVoltage(), vandeGraaffController.ChargeStrength));

#if !UNITY_ANDROID
        // show controls
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[E] - Switch {0} Van de Graaff Generator\r\n[F] - {1} Electric Field\r\n[C] - {2} Charge", vandeGraaffController.On ? "OFF" : "ON", vandeGraaffController.FieldLinesEnabled ? "Hide" : "Show", glowEnabled ? "Hide" : "Show"));
#endif
    }
}
