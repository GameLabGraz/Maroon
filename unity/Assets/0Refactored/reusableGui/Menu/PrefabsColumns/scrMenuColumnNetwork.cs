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

    [SerializeField] private GameObject ColumnSettings;
    
    
    [SerializeField] private GameObject ButtonJoin;

    [SerializeField] private GameObject ButtonHost;
    
    [SerializeField] private GameObject ButtonSettings;
    
    void Start()
    {
        // Link button actions
        this.ButtonJoin.GetComponent<Button>().onClick.AddListener(() => this.OnClickJoin());
        this.ButtonHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickHost());
        this.ButtonSettings.GetComponent<Button>().onClick.AddListener(() => this.OnClickSettings());

        //TODO better way?
        _menu = FindObjectOfType<scrMenu>();
        _networkManager = FindObjectOfType<MaroonNetworkManager>();
        _networkManager.StartMultiUser();
    }

    private void OnClickJoin()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnJoin);
        this.ButtonJoin.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
        this.ButtonHost.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonSettings.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }
    
    private void OnClickHost()
    {
        //TODO: More?
        _networkManager.StartHost();
        _menu.CloseMenu();
    }
    
    private void OnClickSettings()
    {
        //TODO
    }
    
    //TODO: different online and offline
}
