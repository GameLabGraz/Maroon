using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuButtonServer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PingText;
    
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
            NetworkManager.singleton.networkAddress = status.ip;
            NetworkManager.singleton.StartClient();
            //TODO: better?
            FindObjectOfType<scrMenu>().CloseMenu();
        });
        if (status.players >= status.capacity)
        {
            joinButton.interactable = false;
        }
    }
}
