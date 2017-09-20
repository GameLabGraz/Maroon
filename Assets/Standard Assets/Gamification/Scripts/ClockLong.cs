using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockLong : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        if (GamificationManager.instance.gameStarted)
            this.transform.rotation = GamificationManager.instance.clockLongRotation;
      
    }
	
	// Update is called once per frame
	void Update ()
    {
        GamificationManager.instance.clockLongRotation = this.transform.rotation;
    }
}
