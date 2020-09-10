using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class NetworkStatusStation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScreen;

    [SerializeField] private Light statusLight;
    
    private ListServer _ls;
    
    // Start is called before the first frame update
    void Start()
    {
        _ls = FindObjectOfType<ListServer>();
        InvokeRepeating(nameof(UpdateNetworkInformation), 0, 1);
    }

    private void UpdateNetworkInformation()
    {
        bool clientStarted = MaroonNetworkManager.Instance.mode == NetworkManagerMode.ClientOnly;
        bool hostStarted = NetworkServer.active;
        bool listServerConnected = _ls.GetListServerStatus();
        bool portsMapped = MaroonNetworkManager.Instance.PortsMapped;

        if (!MaroonNetworkManager.Instance.IsActive())
        {
            DisplayMessage("Go to the menu to connect to a server or host your own one.");
            return;
        }

        statusLight.gameObject.SetActive(true);
        
        if (clientStarted)
        {
            DisplayOnlineInformation();
            statusLight.color = Color.green;
            return;
        }

        if (!hostStarted)
        {
            if (listServerConnected)
            {
                DisplayMessage("Go to the menu to connect to a server or host your own one.");
                statusLight.color = Color.yellow;
            }
            else
            {
                DisplayMessage("Could not connect to the list server.\nYou can still host or join a local game.");
                statusLight.color = Color.red;
            }
        }
        else
        {
            if (listServerConnected && portsMapped)
            {
                DisplayOnlineInformation();
                statusLight.color = Color.green;
            }
            else if (portsMapped) //List server not connected
            {
                DisplayMessage("Could not connect to the list server.\nUsers can still join you locally.");
                statusLight.color = Color.cyan;
            }
            else //Ports not mapped
            {
                DisplayMessage("Ports could not be automatically forwarded, but <color=#800000>other users can still join you locally</color>.\n" +
                               "Please let another user host the server or contact your system administrator to manually forward the ports.\n" +
                               "When the ports have been mapped manually, you can enable the server over the menu.");
                statusLight.color = Color.cyan;
            }
        }
    }

    private void DisplayOnlineInformation()
    {
        string information = "";
        information += "Hello <color=#800000>" + MaroonNetworkManager.Instance.PlayerName + "</color>!\n";
        information += "You are on the server <color=#800000>" + MaroonNetworkManager.Instance.ServerName + "</color>.\n";
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            information += "<color=#800000>You</color> are currently in control!";
        }
        else
        {
            information += "<color=#800000>" + MaroonNetworkManager.Instance.GetClientInControl() + "</color> is currently in control.";
        }
        DisplayMessage(information);
    }
    
    private void DisplayMessage(string message)
    {
        textScreen.text = message;
    }
}
