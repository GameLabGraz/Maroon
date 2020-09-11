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
            string msg = LanguageManager.Instance.GetString("ScreenHostJoin");
            DisplayMessage(msg);
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
                string msg = LanguageManager.Instance.GetString("ScreenHostJoin");
                DisplayMessage(msg);
            }
            else
            {
                string msg = LanguageManager.Instance.GetString("ScreenNoListServerJoin");
                DisplayMessage(msg);
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
                string msg = LanguageManager.Instance.GetString("ScreenNoListServerHost");
                DisplayMessage(msg);
                statusLight.color = Color.cyan;
            }
            else //Ports not mapped
            {
                string msg = LanguageManager.Instance.GetString("ScreenNoPortsMapped");
                DisplayMessage(msg);
                statusLight.color = Color.cyan;
            }
        }
    }

    private void DisplayOnlineInformation()
    {
        string information = "";
        information += LanguageManager.Instance.GetString("OnlineInformation1") + " <color=#800000>" + MaroonNetworkManager.Instance.PlayerName + "</color>!\n";
        information += LanguageManager.Instance.GetString("OnlineInformation2") + " <color=#800000>" + MaroonNetworkManager.Instance.ServerName + "</color>.\n";
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            information += LanguageManager.Instance.GetString("OnlineInformation3");
        }
        else
        {
            information += "<color=#800000>" + NetworkSyncVariables.Instance.ClientInControl + "</color> " + LanguageManager.Instance.GetString("OnlineInformation4");
        }
        DisplayMessage(information);
    }
    
    private void DisplayMessage(string message)
    {
        textScreen.text = message;
    }
}
