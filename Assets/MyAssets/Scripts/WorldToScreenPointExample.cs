using UnityEngine;
using System.Collections;

public class WorldToScreenPointExample : MonoBehaviour {
    public Transform target;
	
    void Update() 
	{
        Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(target.position);
        print("target is " + screenPos.x + " pixels from the left");
    }
}

