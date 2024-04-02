using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.UI;
using TMPro;
using UnityEngine;

public class StopWatchStartButton : MonoBehaviour
{
    private float hover_timer = 0.0f;
    private bool hovering = false;
    [SerializeField]private SimulationStartButton simStartButton;
    [SerializeField] private TMP_Text timer_countdown;
    private TMP_Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        Debug.Log(text.text);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hover Timer: " + hover_timer.ToString("0.000"));
        if (simStartButton.enabled)
        {
            if (hover_timer >= 3.0f)
            {
                //drop the ball after 3 seconds of hovering
                hovering = false;
                hover_timer = 0.0f;
                simStartButton.StartSimulation();
            }
            
            if (hovering)
            {
                hover_timer = hover_timer + Time.deltaTime;
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
