using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Tools
{
    public class SWEventArgs
    {
        public double SecondsPassed;
        public double MinutesPassed;
        public double MillisecondsPassed;
        public bool IsRunning;
        public Time GameTime = new Time();

        public SWEventArgs(double elapsed, bool running)
        {
            SecondsPassed = elapsed;
            MillisecondsPassed = SecondsPassed * 1000;
            MinutesPassed = SecondsPassed / 60;
            IsRunning = running;
        }
    }

    [Serializable]
    public class StopWatchEvent : UnityEvent<SWEventArgs> { }

    public class StopWatch : MonoBehaviour
    {
        private float _startTime;
        private float _lastElapsed;

        public StopWatchEvent OnStart;
        public StopWatchEvent OnStop;
        public StopWatchEvent OnReset;
        public StopWatchEvent OnTick;

        public bool isRunning { get; private set; }
        public bool isReset { get; private set; }

        public float Elapsed
        {
            get
            {
                if (isReset)
                    return 0;
                if (!isRunning)
                    _startTime = Time.time - _lastElapsed;
                return Time.time - _startTime;
            }
        }

        private void Start()
        {
            isRunning = false;
            SWReset();
        }

        private IEnumerator Run()
        {
            while (isRunning)
            {
                OnTick?.Invoke(new SWEventArgs(Elapsed, isRunning));
                yield return new WaitForSeconds(0.001f);
            }
        }

        public void SWStart()
        {
            if (isRunning && !isReset)
              return;

            isReset = false;
            isRunning = true;

            _startTime = Time.time - _lastElapsed;

            StartCoroutine(Run());

            OnStart?.Invoke(new SWEventArgs(Elapsed, isRunning));
        }

        public void SWStop()
        {
            _lastElapsed = Elapsed;
            isRunning = false;

            OnStop?.Invoke(new SWEventArgs(Elapsed, isRunning));
        }

        public void SWReset()
        {
            isReset = true;

            _lastElapsed = 0;
            _startTime = Time.time;

            OnReset?.Invoke(new SWEventArgs(Elapsed, isRunning));
        }
    }
}

