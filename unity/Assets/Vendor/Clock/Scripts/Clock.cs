using System;
using System.Collections;
using UnityEngine;

namespace Vendor.Clock
{
    public class Clock : MonoBehaviour
    {
        private int hours = 0;
        private int minutes = 0;
        private int seconds = 0;

        [Header("Clock Hands")]
        [SerializeField] private GameObject ClockHandHours;
        [SerializeField] private GameObject ClockHandMinutes;
        [SerializeField] private GameObject ClockHandSeconds;

        private void Start()
        {
            StartCoroutine(UpdateClock());
        }

        private IEnumerator UpdateClock()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                seconds = now.Second;
                minutes = now.Minute;
                hours = now.Hour;

                // calculate clock hand angles
                float rotationHours = ((360.0f / 12.0f) * hours) + ((360.0f / (60.0f * 12.0f)) * minutes);
                float rotationMinutes = (360.0f / 60.0f) * minutes;
                float rotationSeconds = (360.0f / 60.0f) * seconds;

                // set clock hand rotations
                ClockHandHours.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);
                ClockHandMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
                ClockHandSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}