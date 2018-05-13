using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockLong : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        if (GamificationManager.instance.GameStarted)
            this.transform.rotation = GamificationManager.instance.ClockLongRotation;
      
    }
	
	// Update is called once per frame
	void Update ()
    {
        GamificationManager.instance.ClockLongRotation = this.transform.rotation;
    }
}
