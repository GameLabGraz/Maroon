using UnityEngine;
using System.Collections;
using Valve.VR;

public class ColliderEntered : MonoBehaviour {

	public string LevelName;
	public string DisplayedText;	// starts with "Press [E] "
	public bool insideTriggerSphere = false; 
	private GUIStyle textStyle;
    private GameObject level1;
    private GameObject level2;
    private GameObject collidedWith;


    public void Start()
	{
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
        //this.level1 = GameObject.FindGameObjectWithTag("Experiment1");
        //this.level2 = GameObject.FindGameObjectWithTag("Experiment2");
       
    }

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player entered");
			this.insideTriggerSphere = true;
		}

    }

	public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player exit");
			this.insideTriggerSphere = false;
		}
	}

	public void Update()
	{
        /* if (Input.GetKeyDown (KeyCode.E) && this.insideTriggerSphere) {
			Debug.Log(LevelName);
			Application.LoadLevel(LevelName);		
		} desktop version, legacy */
    
        var system = OpenVR.System;
        var left = 0;
        var right = 0;

        // init from SteamVR_ControllerManager.cs 
        if (system != null)  
        {
            // uint leftControllerIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            // uint rightControllerIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand); 

            // other approach to get index from SteamVR_TestController.cs
            left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
            right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

        }
       
        // hint from https://steamcommunity.com/app/250820/discussions/0/481115363874382549/ 
        /*
        if (SteamVR_Controller.Input((int)left).GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) 
            // || SteamVR_Controller.Input((int)right).GetPressDown(SteamVR_Controller.ButtonMask.Trigger )) 
        {
            //hack,fix this later:
           //this.insideTriggerSphere = true;

            Debug.Log("Collider entered,  left trigger pressed");
            // TODO if (this.insideTriggerSphere && ) //only enter level if inside pink sphere

            LevelName = "VandeGraaffExperiment1";
            Debug.Log("Will load " + LevelName);
            Application.LoadLevel(LevelName); //obsolete, but never change a running system ;) 
            //UnityEngine.SceneManagement.SceneManager.LoadScene(LevelName); // for newer versions        
         }

        if (SteamVR_Controller.Input((int)right).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("Collider entered, right trigger pressed");
            // TODO if (this.insideTriggerSphere && ) //only enter level if inside pink sphere

            LevelName = "VandeGraaffExperiment2";
            Debug.Log("Will load " + LevelName);
            Application.LoadLevel(LevelName); //obsolete, but never change a running system ;) 
        }

        if (SteamVR_Controller.Input((int)right).GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("Collider entered, right menu button pressed");
            // TODO if (this.insideTriggerSphere && ) //only enter level if inside pink sphere

            LevelName = "Whiteboard";
            Debug.Log("Will load " + LevelName);
            Application.LoadLevel(LevelName); //obsolete, but never change a running system ;) 
        }

        if (SteamVR_Controller.Input((int)left).GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("Collider entered, left menu button pressed");
            // TODO if (this.insideTriggerSphere && ) //only enter level if inside pink sphere

            LevelName = "Workstation";
            Debug.Log("Will load " + LevelName);
            Application.LoadLevel(LevelName); //obsolete, but never change a running system ;) 
        } */



        // alternative, from https://www.youtube.com/watch?v=h9IJHZgkcME - using SteamVR_TrackedObject TODO


    }

    public void OnGUI()
	{
		if (this.insideTriggerSphere) {
			GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height / 2 - 100f, 400f, 200f), "Press [E] " + DisplayedText, this.textStyle);
		}
	}
}
