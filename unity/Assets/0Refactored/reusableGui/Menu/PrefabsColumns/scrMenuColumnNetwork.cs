using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class scrMenuColumnNetwork : MonoBehaviour
{
    private scrMenu _menu;
    
    [SerializeField] private GameObject ColumnJoin;
    
    [SerializeField] private GameObject ColumnKick;
    
    
    [SerializeField] private GameObject ButtonJoin;

    [SerializeField] private GameObject ButtonHost;
    
    [SerializeField] private GameObject ButtonLeaveClient;
    
    [SerializeField] private GameObject ButtonLeaveHost;
    
    [SerializeField] private GameObject ButtonPortsMapped;
    
    [SerializeField] private GameObject ButtonKick;
    
    void Start()
    {
        // Link button actions
        this.ButtonJoin.GetComponent<Button>().onClick.AddListener(() => this.OnClickJoin());
        this.ButtonHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickHost());
        this.ButtonLeaveClient.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveClient());
        this.ButtonLeaveHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveHost());
        this.ButtonPortsMapped.GetComponent<Button>().onClick.AddListener(() => this.OnClickPortsMapped());
        this.ButtonKick.GetComponent<Button>().onClick.AddListener(() => this.OnButtonKick());

        _menu = FindObjectOfType<scrMenu>();
        MaroonNetworkManager.Instance.StartMultiUser();
        
        ButtonJoin.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonHost.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonLeaveClient.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.ClientOnly);
        ButtonLeaveHost.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Host);
        ButtonPortsMapped.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Host && !MaroonNetworkManager.Instance.PortsMapped);
        ButtonKick.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Host);
    }

    private void OnClickJoin()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnJoin);
        this.ButtonJoin.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
        this.ButtonKick.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
    }
    
    private void OnClickHost()
    {
        MaroonNetworkManager.Instance.StartHostWithPassword();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveClient()
    {
        MaroonNetworkManager.Instance.StopClientRegularly();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveHost()
    {
        MaroonNetworkManager.Instance.StopHost();
        _menu.CloseMenu();
    }

    private void OnClickPortsMapped()
    {
        MaroonNetworkManager.Instance.PortsMapped = true;
        _menu.CloseMenu();
    }

    private void OnButtonKick()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnKick);
        this.ButtonJoin.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.clear;
        this.ButtonKick.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
}
