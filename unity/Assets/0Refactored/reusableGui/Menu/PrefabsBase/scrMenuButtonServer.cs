using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuButtonServer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PingText;

    [SerializeField] private GameObject LocalIcon;
    
    [SerializeField] private TextMeshProUGUI NameText;
    
    [SerializeField] private TextMeshProUGUI PlayerText;

    public void SetServerInfos(ServerStatus status)
    {
        if (status.lastLatency != -1)
        {
            PingText.text = status.lastLatency.ToString() + "\nms";
        }
        else
        {
            PingText.text = "...";
        }

        NameText.text = status.title;

        PlayerText.text = status.players + "/" + status.capacity;

        Button joinButton = GetComponent<Button>();
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            MaroonNetworkManager.Instance.networkAddress = status.ip;
            MaroonNetworkManager.Instance.StartClientWithPassword();
            MaroonNetworkManager.Instance.ServerName = status.title;
            FindObjectOfType<scrMenu>().CloseMenu();
        });
        if (status.players >= status.capacity)
        {
            joinButton.interactable = false;
        }

        if (status.isLocal)
        {
            PingText.gameObject.SetActive(false);
            LocalIcon.SetActive(true);
        }
        else
        {
            PingText.gameObject.SetActive(true);
            LocalIcon.SetActive(false);
        }
    }
}
