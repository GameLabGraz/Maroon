using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;



namespace Maroon.Physics.Viscosimeter
{
    [RequireComponent(typeof(EventTrigger))]
    public class StopWatchStartButton : MonoBehaviour
    {
        private float hover_timer = 0.0f;
        private bool hovering = false;
        [SerializeField] private TMP_Text timer_countdown;
        private TMP_Text text;

        private void Start()
        {
            text = gameObject.GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            if (SimulationController.Instance.enabled)
            {
                if (hover_timer >= 3.0f)
                {
                    //drop the ball after 3 seconds of hovering
                    hovering = false;
                    hover_timer = 0.0f;
                    SimulationController.Instance.StartSimulation();
                    timer_countdown.gameObject.SetActive(false);
                }

                if (hovering)
                {
                    hover_timer += Time.unscaledDeltaTime;
                    text.text = "Dropping in " + (3 - Math.Floor(hover_timer)).ToString("0") + " Seconds";
                    timer_countdown.text = (3 - Math.Floor(hover_timer)).ToString("0");
                }
                else
                {
                    text.text = "Start";
                    timer_countdown.text = "3";
                }
            }
        }

        public void MouseEnter()
        {
            hovering = true;
            timer_countdown.gameObject.SetActive(true);
        }

        public void MouseExit()
        {
            hover_timer = 0f;
            hovering = false;
            timer_countdown.gameObject.SetActive(false);
        }
    }
}