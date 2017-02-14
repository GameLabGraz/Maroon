using UnityEngine;
using System.Collections;

public class MoveLeftRight : MonoBehaviour {

	public float maxMovementLeft;
	public float maxMovementRight;

	private Vector3 initialPosition;
	private float lastDistance;

	public void Start()
	{
		this.initialPosition = transform.position;
        SyncExperiments se = GameObject.FindGameObjectWithTag("SyncExperiments").GetComponent<SyncExperiments>();
        for (int i = 0; i < se.vdg1_dist; i++)
            this.Move(Vector3.right, maxMovementRight);
        for (int i = 0; i > se.vdg1_dist; i--)
            this.Move(Vector3.left,  maxMovementLeft);
    }

	public void Update () 
	{
		//if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) 
		//{
		//	this.Move(Vector3.left, maxMovementLeft);
		//}
		//if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
		//{
		//	this.Move(Vector3.right, maxMovementRight);
		//}
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
