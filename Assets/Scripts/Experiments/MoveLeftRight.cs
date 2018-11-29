using UnityEngine;
using VRTK;

public class MoveLeftRight : MonoBehaviour
{
    [SerializeField]
    private float maxMovementLeft = 0;

    [SerializeField]
	private float maxMovementRight = 0;

    [SerializeField]
    private float movementSpeed = 1;

    [SerializeField]
    private float hapticPulseStrength = 0.5f;

    private Vector3 initialPosition;
	private float lastDistance;

    private GameObject leftController;
    private GameObject rightController;

    public void Start()
	{
		initialPosition = transform.position;

        leftController = VRTK_DeviceFinder.GetControllerLeftHand();
        rightController = VRTK_DeviceFinder.GetControllerRightHand();
    }

    private void Update()
    {       
        if(!leftController)
            leftController = VRTK_DeviceFinder.GetControllerLeftHand();
        if(!rightController)
            rightController = VRTK_DeviceFinder.GetControllerRightHand();

        if (leftController)
        {
            var controllerEvent = leftController.GetComponent<VRTK_ControllerEvents>();
            if(controllerEvent.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripPress))
            {
                Debug.Log("Move grounder to LEFT");
                Move(Vector3.left, maxMovementLeft);

                VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(leftController),
                    hapticPulseStrength);
            }
        }
        if(rightController)
        {
            var controllerEvent = rightController.GetComponent<VRTK_ControllerEvents>();
            if (controllerEvent.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripPress))
            {
                Debug.Log("Move grounder to RIGHT");
                Move(Vector3.right, maxMovementLeft);

                VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(rightController),
                    hapticPulseStrength);
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left, maxMovementLeft);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right, maxMovementRight);
        }
    }

    public void Move(Vector3 direction, float maxMovement)
	{
		if (transform) 
		{
			Vector3 translateVector = direction * Time.deltaTime * movementSpeed;
			Vector3 newPosition = transform.position + transform.TransformDirection(translateVector);
			
			float distance = Vector3.Distance(initialPosition, newPosition);
			if(distance < maxMovement || distance < lastDistance)
            {
				transform.Translate(translateVector);
				lastDistance = distance;
			}
		}
	}
}
