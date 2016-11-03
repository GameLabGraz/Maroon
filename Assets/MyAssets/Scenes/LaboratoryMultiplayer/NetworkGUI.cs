using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManagerHUD))]
public class NetworkGUI : MonoBehaviour {

    private NetworkManagerHUD gui;

	// Use this for initialization
	void Start () {
        gui = GetComponent<NetworkManagerHUD>();
        gui.offsetX = Screen.width - 240;
        gui.offsetY = -20;
    }
}
