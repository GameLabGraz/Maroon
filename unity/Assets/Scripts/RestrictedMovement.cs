using UnityEngine;

public class RestrictedMovement : MonoBehaviour {
    
    public Vector3 MaximumRelativeRotations;
    public Quaternion neutralPosition;
	// Use this for initialization
	void Start () {
        neutralPosition = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        //TODO: z is defenitely wrong...

        float MouseYPercent=  (-Input.mousePosition.y + (Screen.height / 2.0f)) /  (Screen.height / 2.0f);
        float MouseXPercent = (Input.mousePosition.x - (Screen.width / 2.0f)) / (Screen.width / 2.0f);
        
        var nVec = neutralPosition.eulerAngles;
        transform.rotation = Quaternion.Euler(
                nVec.x + MaximumRelativeRotations.x * MouseYPercent,
                nVec.y + MaximumRelativeRotations.y * MouseXPercent,
                nVec.z
        );
        
	}
}
