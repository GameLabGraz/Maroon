using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerEventListener : MonoBehaviour
{
    private SteamVR_TrackedController controller;

    private void Awake ()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.Gripped += HandleGripped;
        controller.Ungripped += HandleUnGripped;
        controller.MenuButtonClicked += HandleMenuButtonClicked;
        controller.MenuButtonUnclicked += HandleMenuButtonUnclicked;
        controller.PadClicked += HandlePadClicked;
        controller.PadUnclicked += HandlePadUnclicked;
        controller.PadTouched += HandlePadTouched;
        controller.PadUntouched += HandlePadUntouched;
        controller.SteamClicked += HandleSteamClicked;
        controller.TriggerClicked += HandleTriggerClicked;
        controller.TriggerUnclicked += HandleTriggerUnclicked;
    }

    private void OnDestroy()
    {
        controller.Gripped -= HandleGripped;
        controller.Ungripped -= HandleUnGripped;
        controller.MenuButtonClicked -= HandleMenuButtonClicked;
        controller.MenuButtonUnclicked -= HandleMenuButtonUnclicked;
        controller.PadClicked -= HandlePadClicked;
        controller.PadUnclicked -= HandlePadUnclicked;
        controller.PadTouched -= HandlePadTouched;
        controller.PadUntouched -= HandlePadUntouched;
        controller.SteamClicked -= HandleSteamClicked;
        controller.TriggerClicked -= HandleTriggerClicked;
        controller.TriggerUnclicked -= HandleTriggerUnclicked;
    }

    private void HandleGripped(object sender, ClickedEventArgs e)
    {

    }

    private void HandleUnGripped(object sender, ClickedEventArgs e)
    {

    }

    private void HandleMenuButtonClicked(object sender, ClickedEventArgs e)
    {
        SceneManager.LoadScene("Laboratory.vr");
    }

    private void HandleMenuButtonUnclicked(object sender, ClickedEventArgs e)
    {

    }

    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        // Enable Teleport
        
    }

    private void HandlePadUnclicked(object sender, ClickedEventArgs e)
    {
        // Teleport
    }

    private void HandlePadTouched(object sender, ClickedEventArgs e)
    {

    }

    private void HandlePadUntouched(object sender, ClickedEventArgs e)
    {

    }

    private void HandleSteamClicked(object sender, ClickedEventArgs e)
    {

    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {

    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {

    }
}
