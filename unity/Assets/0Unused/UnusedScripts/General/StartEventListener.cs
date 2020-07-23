using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class StartEventListener : MonoBehaviour
{
    private void Start()
    {
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        // TODO fix null reference 
        GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler(DoButtonPressed);

    }

    private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action
                + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerReference.index, "TRIGGER", "pressed down", e);
       
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerReference.index, "TRIGGER", "released", e);
    }

    //TODO
    private void DoButtonPressed(object sender, InteractableObjectEventArgs e)
    {
        Debug.Log("registered button use");
        string LevelName = "VandeGraaffExperiment2";
        Debug.Log("Will load " + LevelName);
        SceneManager.LoadScene(LevelName);
    }

}