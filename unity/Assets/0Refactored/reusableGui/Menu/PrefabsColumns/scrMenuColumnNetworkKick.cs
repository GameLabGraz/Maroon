using System.Collections;
using System.Collections.Generic;
using GEAR.Localization.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuColumnNetworkKick : MonoBehaviour
{
    [SerializeField] private GameObject ButtonPlayer;
    
    [SerializeField] private RectTransform ScrollContent;
    
    [SerializeField] private Texture TextureIconHost;
    
    [SerializeField] private Texture TextureIconKick;

    private Dictionary<string, GameObject> _playerButtons = new Dictionary<string, GameObject>();
    
    void Start()
    {
        List<string> orderedPlayers = MaroonNetworkManager.Instance.ServerGetPlayerNames();
        orderedPlayers.Sort();

        foreach (var player in orderedPlayers)
        {
            AddButton(player);
        }
    }

    private void AddButton(string playerName)
    {
        GameObject new_button;
        new_button = Instantiate(ButtonPlayer) as GameObject;
        new_button.GetComponentInChildren<LocalizedTMP>().enabled = false;
        new_button.GetComponentInChildren<TextMeshProUGUI>().text = playerName;
        
        if (playerName == MaroonNetworkManager.Instance.PlayerName)
        {
            new_button.GetComponent<Button>().interactable = false;
            new_button.transform.Find("IconContainer").Find("Icon").GetComponent<RawImage>().texture = TextureIconHost;
        }
        else
        {
            new_button.GetComponent<Button>().onClick.AddListener(() => this.KickPlayer(playerName));
            new_button.transform.Find("IconContainer").Find("Icon").GetComponent<RawImage>().texture = TextureIconKick;
        }
        
        new_button.transform.SetParent(this.ScrollContent, false);
        _playerButtons[playerName] = new_button;
    }

    private void KickPlayer(string playerName)
    {
        MaroonNetworkManager.Instance.KickPlayer(playerName);
        Destroy(_playerButtons[playerName]);
    }
}
