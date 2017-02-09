using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkManagerHUD))]
public class NetworkGUI : NetworkBehaviour {

    private NetworkManagerHUD gui;
    public Button b;

    // Use this for initialization
    void Start () {
        gui = GetComponent<NetworkManagerHUD>();
        gui.offsetX = 0;
        gui.offsetY = -10;
        b.onClick.AddListener(click);
    }

    void click()
    {
        gui.showGUI = !gui.showGUI;
    }
}
