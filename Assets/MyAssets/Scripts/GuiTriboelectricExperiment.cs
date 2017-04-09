using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GuiTriboelectricExperiment : MonoBehaviour {
	
	private GUIStyle textStyle;
    private int left = -1;
    private int right = -1;
	
	public void Start () {
		// define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;

        var system = Valve.VR.OpenVR.System;
        if (system != null)
        {
            left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
            right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        }
    }
	
	public void Update()
	{
        // check if [ESC] was pressed
        // VIVE -> ESC == MENU BUTTON top 
        // DESKTOP if (Input.GetKeyDown (KeyCode.Escape))  
        if (SteamVR_Controller.Input((int)left).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu) ||
         SteamVR_Controller.Input((int)right).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            SceneManager.LoadScene("Laboratory");
        }
	}
	
	// OnGUI is called once per frame
	public void OnGUI()
	{
		GUI.Label (new Rect (Screen.width / 2 - 200f, Screen.height / 3, 400f, 100f), "Sorry, this experiment is not ready yet.", this.textStyle);
		// show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave"));
	}
}
