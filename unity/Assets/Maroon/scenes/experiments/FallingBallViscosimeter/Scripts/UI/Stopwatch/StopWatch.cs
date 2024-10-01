using TMPro;
using UnityEngine;

//Done with tutorial from https://medium.com/@eveciana21/creating-a-stopwatch-timer-in-unity-f4dff748030d



namespace Maroon.Physics.Viscosimeter
{

    public class StopWatch : MonoBehaviour
    {
        private bool timer_running;
        private float current_time;
        [SerializeField] private TMP_Text timer_display;

        void Start()
        {
            current_time = 0.0f;
            timer_running = false;
        }


        void Update()
        {
            if (timer_running)
            {
                current_time = current_time + Time.deltaTime;
            }

            timer_display.text = current_time.ToString("0.000");
        }

        public void StartTimer()
        {
            timer_running = true;
        }

        public void PauseTimer()
        {
            timer_running = false;
        }

        public void ResetTimer()
        {
            timer_running = false;
            current_time = 0f;
        }
    }
}