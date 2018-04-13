using UnityEngine;
using UnityEngine.VR;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerVR;

    [SerializeField]
    private GameObject PlayerSimulator;

    [SerializeField]
    private bool preferSimulator = false;

    private void Awake()
    {
        if(!preferSimulator && VRDevice.isPresent)
        {
            PlayerVR.SetActive(true);
            PlayerSimulator.SetActive(false);
        }
        else
        {
            PlayerVR.SetActive(false);
            PlayerSimulator.SetActive(true);
        }
    }
}
