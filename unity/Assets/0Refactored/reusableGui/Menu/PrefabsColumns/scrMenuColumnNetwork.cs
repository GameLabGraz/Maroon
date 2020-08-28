using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class scrMenuColumnNetwork : MonoBehaviour
{
    private scrMenu _menu;
    private MaroonNetworkManager _networkManager;
    
    [SerializeField] private GameObject ColumnJoin;
    
    
    [SerializeField] private GameObject ButtonJoin;

    [SerializeField] private GameObject ButtonHost;
    
    [SerializeField] private GameObject ButtonLeaveClient;
    
    [SerializeField] private GameObject ButtonLeaveHost;
    
    void Start()
    {
        // Link button actions
        this.ButtonJoin.GetComponent<Button>().onClick.AddListener(() => this.OnClickJoin());
        this.ButtonHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickHost());
        this.ButtonLeaveClient.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveClient());
        this.ButtonLeaveHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveHost());

        //TODO better way?
        _menu = FindObjectOfType<scrMenu>();
        _networkManager = FindObjectOfType<MaroonNetworkManager>();
        _networkManager.StartMultiUser();
        
        ButtonJoin.SetActive(_networkManager.mode == NetworkManagerMode.Offline);
        ButtonHost.SetActive(_networkManager.mode == NetworkManagerMode.Offline);
        ButtonLeaveClient.SetActive(_networkManager.mode == NetworkManagerMode.ClientOnly);
        ButtonLeaveHost.SetActive(_networkManager.mode == NetworkManagerMode.Host);
    }

    private void OnClickJoin()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnJoin);
        this.ButtonJoin.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
    
    private void OnClickHost()
    {
        _networkManager.StartHost();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveClient()
    {
        _networkManager.StopClient();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveHost()
    {
        _networkManager.StopHost();
        _menu.CloseMenu();
    }
}
