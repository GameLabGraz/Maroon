using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSmall : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (GamificationManager.instance.gameStarted)
             this.transform.rotation = GamificationManager.instance.clockSmallRotation;

    }

    // Update is called once per frame
    void Update()
    {
        GamificationManager.instance.clockSmallRotation = this.transform.rotation;
    }
}
