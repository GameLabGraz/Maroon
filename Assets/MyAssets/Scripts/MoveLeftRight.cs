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
                this.Move(Vector3.left, maxMovementLeft);

                // trigger haptic pulse     
                SteamVR_Controller.Input(left).TriggerHapticPulse(3999);
                
                Debug.Log("moved grounder to LEFT");
             } 
        }

        if (SteamVR_Controller.Input((int)right).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("RIGHT grips pressed");

            grounder = GameObject.Find("Grounder");
            if(grounder != null)
            {
                this.Move(Vector3.right, maxMovementRight);

                SteamVR_Controller.Input(right).TriggerHapticPulse(3999);

                Debug.Log("moved grounder to RIGHT");
            }
        }
    }

    public void Move(Vector3 direction, float maxMovement)
	{
		if (null != transform) 
		{
			Vector3 translateVector = direction * Time.deltaTime * movementSpeed;
			Vector3 newPosition = transform.position + transform.TransformDirection(translateVector);
			
			float distance = Vector3.Distance(this.initialPosition, newPosition);
			if(distance < (maxMovement) || distance < this.lastDistance) {
				transform.Translate(translateVector);
				this.lastDistance = distance;
			}
		}
	}
}
