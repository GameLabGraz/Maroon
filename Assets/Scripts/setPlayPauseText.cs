using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setPlayPauseText : MonoBehaviour {
    [SerializeField]
    private Text PlayPauseText;
    [SerializeField]
    private bool simulationRunning = false;
    // Use this for initialization
    void Start () {
        if (PlayPauseText == null)
            PlayPauseText = this.transform.GetComponent<Text>();

	}

    public void ToggleSimulation()
    {
        if (PlayPauseText == null)
        {
            Debug.LogWarning("[setPlayPauseText] Cannot set Stop/Pay- Text");
            return;
        }
        simulationRunning = !simulationRunning;
        Debug.Log("[setPlayPauseText] Stop Play Button pressed, will display " + (simulationRunning ? "stop" : "play"));
        PlayPauseText.text = simulationRunning ? "PAUSE\nSimulation" : "PLAY\nSimulation";
    }
}
