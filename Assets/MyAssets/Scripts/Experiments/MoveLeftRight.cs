using UnityEngine;

public class MoveLeftRight : MonoBehaviour
{

    [SerializeField]
    private GameObject grounder;

    [SerializeField]
    public float maxMovementLeft;

    [SerializeField]
	public float maxMovementRight;

    [SerializeField]
    public float movementSpeed = 10;
   
    private Vector3 initialPosition;
	private float lastDistance;

    private int controllerLeftIndex;
    private int controllerRightIndex;

    public void Start()
	{
		this.initialPosition = transform.position;

        if(grounder == null)
            grounder = GameObject.Find("Grounder"); //important to avoid null reference

        // VIVE: use grip on left controller to move left, grip on right controller to move right
        var system = Valve.VR.OpenVR.System;

        if (system != null)
        {
            controllerLeftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost); //spiegelverkehrte Ansicht, aus Sicht des PCs?!
            controllerRightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost); // switching so it makes sense from user POV
        }
    }

    public void Update()
    {
        if (controllerLeftIndex == -1)
            controllerLeftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);

        if (controllerRightIndex == -1)
            controllerRightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);


        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            this.Move(Vector3.left, maxMovementLeft);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            this.Move(Vector3.right, maxMovementRight);
        } 

        if (controllerLeftIndex != -1 && SteamVR_Controller.Input(controllerLeftIndex).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("LEFT grips pressed");


            if(grounder != null)
            {
                this.Move(Vector3.left, maxMovementLeft);

                // trigger haptic pulse     
                SteamVR_Controller.Input(controllerLeftIndex).TriggerHapticPulse(3999);
                
                Debug.Log("moved grounder to LEFT");
             } 
        }

        if (controllerRightIndex != -1 && SteamVR_Controller.Input(controllerRightIndex).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("RIGHT grips pressed");

            grounder = GameObject.Find("Grounder");
            if(grounder != null)
            {
                this.Move(Vector3.right, maxMovementRight);

                SteamVR_Controller.Input(controllerRightIndex).TriggerHapticPulse(3999);

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
