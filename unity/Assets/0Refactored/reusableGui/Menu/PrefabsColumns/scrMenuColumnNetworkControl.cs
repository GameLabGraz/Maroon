using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Localization;
using Mirror;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnNetworkControl : MonoBehaviour
{
    [SerializeField] private GameObject ControlButton;
    
    [SerializeField] private RectTransform ScrollContent;
    
    [SerializeField] private Texture TextureIconBase;

    [SerializeField] private Texture TextureIconSelected;

    private void Start()
    {
        foreach (var playerName in NetworkSyncVariables.Instance.connectedPlayers)
        {
            AddPlayerButton(playerName);
        }
        //TODO: Update this in Menu?
    }
    
    void AddPlayerButton(string playerName)
    {
        GameObject new_button;
        new_button = Instantiate(ControlButton, this.ScrollContent, false) as GameObject;
        new_button.GetComponent<Button>().onClick.AddListener(() => this.OnClickPlayerButton(playerName));
        new_button.GetComponentInChildren<LocalizedTMP>().enabled = false;
        new_button.GetComponentInChildren<TextMeshProUGUI>().text = playerName;
        var icon = new_button.transform.Find("IconContainer").Find("Icon").GetComponent<RawImage>();
        if (playerName == NetworkSyncVariables.Instance.clientInControl)
        {
            new_button.GetComponent<Button>().interactable = false;
            icon.texture = TextureIconSelected;
        }
        else
        {
            icon.texture = TextureIconBase;
        }
    }

    void OnClickPlayerButton(string playerName)
    {
        NetworkSyncVariables.Instance.GiveControlTo(playerName);
        FindObjectOfType<scrMenu>().CloseMenu();
    }
}
