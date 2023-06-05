using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using Mirror;
using TMPro;
using UnityEngine;

public class NetworkStatusStation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScreen;

    [SerializeField] private Light statusLight;
    [SerializeField] private bool isActive = true;
    
    private ListServer _ls;
    
    // Start is called before the first frame update
    void Start()
    {
        _ls = FindObjectOfType<ListServer>();

        if (isActive)
        {
            InvokeRepeating(nameof(UpdateNetworkInformation), 0, 1);
        }
    }

    private void UpdateNetworkInformation()
    {        
        if (!Maroon.NetworkManager.Instance.IsActive())
        {
            string msg = LanguageManager.Instance.GetString("ScreenHostJoin");
            DisplayMessage(msg);
            return;
        }
        
        bool clientStarted = Maroon.NetworkManager.Instance.mode == NetworkManagerMode.ClientOnly;
        bool hostStarted = NetworkServer.active;
        bool listServerConnected = _ls.GetListServerStatus();
        bool portsMapped = Maroon.NetworkManager.Instance.PortsMapped;
        
        if (clientStarted)
        {
            DisplayOnlineInformation();
            statusLight.color = Color.green;
            statusLight.gameObject.SetActive(true);
            Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.ClientOnline;
            return;
        }

        if (!hostStarted)
        {
            statusLight.gameObject.SetActive(false);
            if (listServerConnected)
            {
                Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.Offline;
                string msg = LanguageManager.Instance.GetString("ScreenHostJoin");
                DisplayMessage(msg);
            }
            else
            {
                Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.OfflineNoConnectionToListServer;
                string msg = LanguageManager.Instance.GetString("ScreenNoListServerJoin");
                DisplayMessage(msg);
            }
        }
        else
        {
            statusLight.gameObject.SetActive(true);
            if (listServerConnected && portsMapped)
            {
                Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.HostOnline;
                DisplayOnlineInformation();
                statusLight.color = Color.green;
            }
            else if (portsMapped) //List server not connected
            {
                Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.HostOnlineNoConnectionToListServer;
                string msg = LanguageManager.Instance.GetString("ScreenNoListServerHost");
                DisplayMessage(msg);
                statusLight.color = Color.red;
            }
            else //Ports not mapped
            {
                Maroon.NetworkManager.Instance.ConnectionStatus = ConnectionState.HostOnlinePortsNotMapped;
                string msg = LanguageManager.Instance.GetString("ScreenNoPortsMapped");
                DisplayMessage(msg);
                statusLight.color = Color.yellow;
            }
        }
    }

    private void DisplayOnlineInformation()
    {
        string information = "";
        information += LanguageManager.Instance.GetString("OnlineInformation1") + " <color=#800000>" + Maroon.NetworkManager.Instance.PlayerName + "</color>!\n";
        information += LanguageManager.Instance.GetString("OnlineInformation2") + " <color=#800000>" + Maroon.NetworkManager.Instance.ServerName + "</color>.\n";
        if (Maroon.NetworkManager.Instance.IsInControl)
        {
            information += LanguageManager.Instance.GetString("OnlineInformation3");
        }
        else
        {
            information += "<color=#800000>" + Maroon.NetworkManager.Instance.ClientInControl + "</color> " + LanguageManager.Instance.GetString("OnlineInformation4");
        }
        DisplayMessage(information);
    }
    
    private void DisplayMessage(string message)
    {
        textScreen.text = message;
    }
}
