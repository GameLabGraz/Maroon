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
        Maroon.NetworkManager.Instance.StartMultiUser();
        
        ButtonJoin.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonHost.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.Offline);
        ButtonLeaveClient.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.ClientOnly);
        ButtonLeaveHost.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.Host);
        ButtonPortsMapped.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.Host && !Maroon.NetworkManager.Instance.PortsMapped);
        ButtonKick.SetActive(Maroon.NetworkManager.Instance.mode == NetworkManagerMode.Host);
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
        Maroon.NetworkManager.Instance.StartHostWithPassword();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveClient()
    {
        Maroon.NetworkManager.Instance.StopClientRegularly();
        _menu.CloseMenu();
    }
    
    private void OnClickLeaveHost()
    {
        Maroon.NetworkManager.Instance.StopHost();
        _menu.CloseMenu();
    }

    private void OnClickPortsMapped()
    {
        Maroon.NetworkManager.Instance.PortsMapped = true;
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
