using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrMenuColumnNetworkJoin : MonoBehaviour
{
    [SerializeField] private GameObject ButtonServer;
    
    [SerializeField] private RectTransform ScrollContent;
    
    [SerializeField] private GameObject NoServerText;

    private ListServer _listServer;
    private Dictionary<string, scrMenuButtonServer> _serverButtons = new Dictionary<string, scrMenuButtonServer>();
    
    void Start()
    {
        _listServer = FindObjectOfType<ListServer>();
        //cannot use InvokeRepeating(nameof(UpdateServerList), 0, 1); because affected by time scale
        UpdateServerList();
        StartCoroutine(InvokeRealtimeCoroutine(0.7f));
    }
    
    private IEnumerator InvokeRealtimeCoroutine(float seconds)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(seconds);
            UpdateServerList();
        }
    }

    void AddServerButton(ServerStatus status)
    {
        GameObject new_button;
        new_button = Instantiate(ButtonServer) as GameObject;
        scrMenuButtonServer new_button_script = new_button.GetComponent<scrMenuButtonServer>();
        new_button_script.SetServerInfos(status);
        new_button.transform.SetParent(this.ScrollContent, false);
        
        _serverButtons[status.ip] = new_button_script;
    }

    void UpdateServerList()
    {
        Debug.Log("Update");
        if (_listServer.list.Count > 0)
        {
            NoServerText.SetActive(false);
        }
        foreach (var server in _listServer.list.Values)
        {
            if (_serverButtons.ContainsKey(server.ip))
            {
                _serverButtons[server.ip].SetServerInfos(server);
            }
            else
            {
                AddServerButton(server);
            }
        }
    }
}
