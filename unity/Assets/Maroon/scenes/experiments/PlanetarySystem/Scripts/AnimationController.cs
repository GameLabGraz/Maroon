using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class AnimationController : MonoBehaviour
{
    public Toggle toggleButton;
    public GameObject togglePlanet;
    public GameHandler gameHandler;

    private void Start()
    {
        toggleButton.onValueChanged.AddListener(UItoggleIsKinematic);
    }

    /*
     * toggle suns isKinematic after the UI togglebButton is pressed
     */
    public void UItoggleIsKinematic(bool isOn)
    {
        //Debug.Log("current checkbox state: neptune: " + !checkmark);
        //neptune.GetComponent<Renderer>().enabled = !checkmark;
        Rigidbody rb = togglePlanet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = isOn;
        }
    }

    private void OnDestroy()
    {
        toggleButton.onValueChanged.RemoveListener(UItoggleIsKinematic);
    }
}
