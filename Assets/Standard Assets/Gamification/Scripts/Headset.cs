using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headset : MonoBehaviour {

    private Transform headset;

	// Use this for initialization
	void Start ()
    {
		headset = GameObject.FindGameObjectWithTag("Headset").transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GamificationManager.instance.Headset)
            headset.localScale = new Vector3(0, 0, 0);
        else
            headset.localScale = new Vector3(7, 7, 7);
       

    }
}
