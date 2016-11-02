using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour {

    public GameObject avatar_;

	void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponentInChildren<FirstPersonController>().enabled = false;
            GetComponentInChildren<AudioSource>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<CharacterController>().enabled = false;
        }
        else
        {
            avatar_.SetActive(false);
        }
    }

}
