using UnityEngine;
using System.Collections;
using VRTK;

public class MoveLeftRight : MonoBehaviour {


    public GameObject grounder;
    public float maxMovementLeft;
	public float maxMovementRight;
    public float movementSpeed;
   
    private Vector3 initialPosition;
	private float lastDistance;
    //public Vector3 newPosition;
    public Animator Anim;
    public SteamVR_Controller.Device device;
    //public VRTK_ControllerActions controllerActions;
    

    public void Start()
	{
		this.initialPosition = transform.position;
        Anim = GetComponent<Animator>();
        Anim.enabled = true;

        movementSpeed = 10;
    }

    public void Update()
    {
        /*if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            this.Move(Vector3.left, maxMovementLeft);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            this.Move(Vector3.right, maxMovementRight);
        } */

        // VIVE: use grip on left controller to move left, grip on right controller to move right
        var system = Valve.VR.OpenVR.System;
        var left = 0;
        var right = 0;
        float z;
        float x;
        float y;
        float z_new;

        if (system != null)
        {
            left = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost); //spiegelverkehrte Ansicht, aus Sicht des PCs?!
            right = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost); // switching so it makes sense from user POV
        }

        if (SteamVR_Controller.Input((int)left).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("LEFT grips pressed");

            grounder = GameObject.Find("Grounder"); //important to avoid null reference
            if(grounder != null)
            {
                x = grounder.transform.position.x;
                y = grounder.transform.position.y;
                z = grounder.transform.position.z;
                z_new = z + maxMovementLeft;

                //https://docs.unity3d.com/ScriptReference/Transform.Translate.html 
                //transform.Translate(Vector3.up * Time.deltaTime, Space.World);
                Vector3 newLeftPosition = new Vector3(x, y, z_new); ;
                this.transform.Translate(Vector3.back * movementSpeed * Time.deltaTime, Space.World);

                // trigger haptic pulse     
                SteamVR_Controller.Input(left).TriggerHapticPulse(3999);
                
                Debug.Log("moved grounder to LEFT");

                /* long version
                device = SteamVR_Controller.Input(left);
                if(device != null)
                {
                    device.TriggerHapticPulse(3999);
                    Debug.Log("trigger haptic pulse");
                }
                else
                {
                    Debug.Log("MoveLeftRight could not trigger haptic pulse");
                }             

                //VRTK version
                controllerActions = device.GetComponent<VRTK_ControllerActions>();
                if (controllerActions != null)
                {
                    controllerActions.TriggerHapticPulse(1000); // public void TriggerHapticPulse(ushort strength)
                }
                else
                {
                    Debug.Log("MoveLeftRight could not find controllerActions");
                }*/

            } 
        }

        if (SteamVR_Controller.Input((int)right).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("RIGHT grips pressed");

            grounder = GameObject.Find("Grounder");
            if(grounder != null)
            {
                x = grounder.transform.position.x;
                y = grounder.transform.position.y;
                z = grounder.transform.position.z;
                z_new = z + maxMovementRight;
                Vector3 newRightPosition = new Vector3(x, y, -z_new); // minus sign to move into opposite direction
                this.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.World); 
            }

            SteamVR_Controller.Input(right).TriggerHapticPulse(3999);

            Debug.Log("moved grounder to RIGHT");
        }
    }

    public void Move(Vector3 direction, float maxMovement)
	{
		if (null != transform) 
		{
			Vector3 translateVector = direction * Time.deltaTime * 0.35f;
			Vector3 newPosition = transform.position + Camera.main.transform.TransformDirection(translateVector);
			
			float distance = Vector3.Distance(this.initialPosition, newPosition);
			if(distance < (maxMovement) || distance < this.lastDistance) {
				transform.Translate(translateVector, Camera.main.transform);
				this.lastDistance = distance;
			}
		}
	}
}
