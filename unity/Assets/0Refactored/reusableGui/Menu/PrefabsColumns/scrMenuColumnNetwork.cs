using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class scrMenuColumnNetwork : MonoBehaviour
{
    private scrMenu _menu;
    
    [SerializeField] private GameObject ColumnJoin;
    
    [SerializeField] private GameObject ColumnControl;
    
    
    [SerializeField] private GameObject ButtonJoin;

    [SerializeField] private GameObject ButtonHost;
    
    [SerializeField] private GameObject ButtonTakeControl;
    
    [SerializeField] private GameObject ButtonGrantControl;
    
    [SerializeField] private GameObject ButtonLeaveClient;
    
    [SerializeField] private GameObject ButtonLeaveHost;
    
    void Start()
    {
        // Link button actions
        this.ButtonJoin.GetComponent<Button>().onClick.AddListener(() => this.OnClickJoin());
        this.ButtonHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickHost());
        this.ButtonTakeControl.GetComponent<Button>().onClick.AddListener(() => this.OnClickTakeControl());
        this.ButtonGrantControl.GetComponent<Button>().onClick.AddListener(() => this.OnClickGrantControl());
        this.ButtonLeaveClient.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveClient());
        this.ButtonLeaveHost.GetComponent<Button>().onClick.AddListener(() => this.OnClickLeaveHost());

        //TODO better way?
        _menu = FindObjectOfType<scrMenu>();
        MaroonNetworkManager.Instance.StartMultiUser();
        
        ButtonJoin.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonHost.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonTakeControl.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Host && !MaroonNetworkManager.Instance.IsInControl);
        ButtonGrantControl.SetActive(MaroonNetworkManager.Instance.IsInControl && NetworkClient.active);
        ButtonLeaveClient.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.ClientOnly);
        ButtonLeaveHost.SetActive(MaroonNetworkManager.Instance.mode == NetworkManagerMode.Host);
    }

    private void OnClickJoin()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnJoin);
        this.ButtonJoin.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
    
    private void OnClickHost()
    {
        MaroonNetworkManager.Instance.StartHost();
        _menu.CloseMenu();
    }
    
    private void OnClickTakeControl()
    {
        MaroonNetworkManager.Instance.TakeControl();
        _menu.CloseMenu();
    }
    
    private void OnClickGrantControl()
    {
        this._menu.RemoveAllMenuColumnsButTwo();
        this._menu.AddMenuColumn(this.ColumnControl);
        this.ButtonGrantControl.transform.Find("IconActiveContainer").Find("Icon").GetComponent<RawImage>().color = Color.white;
    }
    
    private void OnClickLeaveClient()
    {
        MaroonNetworkManager.Instance.StopClient();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveHost()
    {
        MaroonNetworkManager.Instance.StopHost();
        _menu.CloseMenu();
    }
}
