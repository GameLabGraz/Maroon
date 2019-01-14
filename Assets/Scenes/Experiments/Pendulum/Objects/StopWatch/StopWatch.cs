using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Evaluation.UnityInterface;

public class StopWatch : MonoBehaviour {

    [SerializeField]
    private GameObject MinuteHand;
    [SerializeField]
    private GameObject SecondsHand;
    [SerializeField]
    private GameObject TextDisplay = null;


    public delegate void  StateChange(StopWatchEvent evt);
    public static event StateChange OnStart;
    public static event StateChange OnStop;
    public static event StateChange OnReset;
    public static event StateChange OnTick;

    private float lastElapsed;

    public class StopWatchEvent
    {
        public double SecondsPassed = 0;
        public double MinutesPassed = 0;
        public double MillisecondsPassed = 0;
        public bool isRunning = false;
        public DateTime SystemTime = DateTime.Now;
        public Time GameTime = new Time();

        public StopWatchEvent(double elapsed, bool running)
        {
            SecondsPassed = elapsed;
            MillisecondsPassed = SecondsPassed * 1000;
            MinutesPassed = SecondsPassed / 60;
        }
    }

    public Boolean isRunning { get; private set; }
    public Boolean isReset { get; private set; }

    public double Elapsed {  get {
            if (isReset)
                return 0;
            if (!isRunning)
                startTime = Time.time - lastElapsed;

            return Time.time - startTime;
        }
    }
    private float startTime;


    // Use this for initialization
    void Start () {
        isRunning = false;
        SWReset();
	}

    // Update is called once per frame
    void Update() {
       if (!isRunning)
            return;

        isReset = false;
        var evt = new StopWatchEvent(Elapsed, isRunning);
        SetSecondsHand(evt.MillisecondsPassed);
        SetMinutesHand(evt.MinutesPassed);
        SetText(evt.SecondsPassed);

        if (OnTick != null)
            OnTick(evt);
    }

    public void SWStart()
    {
        isRunning = true;
        startTime = Time.time - lastElapsed;

        if (OnStart != null)
            OnStart(new StopWatchEvent(Elapsed, isRunning));
    }

    public void SWStop()
    {
        isRunning = false;
        lastElapsed = (float)Elapsed;

        if (OnStop != null)
            OnStop(new StopWatchEvent(Elapsed, isRunning));
    }
    public void SWReset()
    {
        isReset = true;
        if (OnReset != null)
            OnReset(new StopWatchEvent(Elapsed, isRunning));

        lastElapsed = 0;
        startTime = Time.time;
        SetSecondsHand(0);
        SetMinutesHand(0);
        SetText(0);
    }

    private void SetText(double number)
    {
        if (!TextDisplay)
            return;

        var text = String.Format("{0:0.00}", number);
        TextDisplay.GetComponent<TextMesh>().text = text.Substring(0, Math.Min(5, text.Length));
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
