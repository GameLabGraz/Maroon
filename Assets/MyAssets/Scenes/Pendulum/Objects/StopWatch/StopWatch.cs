using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StopWatch : MonoBehaviour {

    public GameObject MinuteHand;
    public GameObject SecondsHand;
    public GameObject TextDisplay = null;

    private Boolean running;
    private DateTime startTime;


    // Use this for initialization
    void Start () {
        running = false;
        SWReset();
                
	}

    // Update is called once per frame
    void Update() {
        if (!running)
            return;

        TimeSpan passed = DateTime.Now - startTime;
        SetSecondsHand(passed.TotalMilliseconds);
        SetMinutesHand(passed.TotalMinutes);
        SetText(passed.TotalSeconds);
    }

    public void SWReset()
    {
        startTime = DateTime.Now;
        SetSecondsHand(0);
        SetMinutesHand(0);
        SetText(0);
    }

    public void SWStart()
    {
        running = true;
        startTime = DateTime.Now;
    }

    public void SWStop()
    {
        running = false;
    }
    
    private void SetText(double number)
    {
        if (!TextDisplay)
            return;

        var text = String.Format("{0:0.00}", number);
        TextDisplay.GetComponent<TextMesh>().text = text.Substring(0, 4);
    }

    private void SetSecondsHand(double passedMilliseconds)
    {
        if (!SecondsHand)
            return;

        float angle = ((float)passedMilliseconds) * (360f / 60000f);
        SecondsHand.transform.localRotation = Quaternion.Euler(-angle, 0, 0);
    }

    private void SetMinutesHand(double passedMinutes)
    {
        if (!MinuteHand)
            return;

        float angle = ((float)passedMinutes) * (360f / 60f);
        MinuteHand.transform.localRotation = Quaternion.Euler(-angle, 0, 0);
    }

}
