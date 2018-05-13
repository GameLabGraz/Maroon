using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSmall : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (GamificationManager.instance.GameStarted)
             this.transform.rotation = GamificationManager.instance.ClockSmallRotation;

    }

    // Update is called once per frame
    void Update()
    {
        GamificationManager.instance.ClockSmallRotation = this.transform.rotation;
    }
}
