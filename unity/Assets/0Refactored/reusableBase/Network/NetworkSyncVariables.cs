using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[System.Serializable]
public class SyncListNames : SyncList<string> {}

public class NetworkSyncVariables : NetworkBehaviour
{
    [HideInInspector]
    public static NetworkSyncVariables Instance = null;
    
    // Start is called before the first frame update
    [SyncVar] private string clientInControl;
    
    private readonly SyncListNames connectedPlayers = new SyncListNames();

    public string ClientInControl => clientInControl;

    public SyncListNames ConnectedPlayers => connectedPlayers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if(isServer)
            StartCoroutine(InvokeRealtimeTickCoroutine(1));
    }

    private IEnumerator InvokeRealtimeTickCoroutine(float seconds)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(seconds);
            UpdateValuesOnServer();
        }
    }

    private void UpdateValuesOnServer()
    {
        clientInControl = MaroonNetworkManager.Instance.GetClientInControl();
        connectedPlayers.Clear();
        foreach (var pName in MaroonNetworkManager.Instance.GetAllPlayerNames())
        {
            connectedPlayers.Add(pName);
        }
    }

    public void GiveControlTo(string playerName)
    {
        CmdGiveControl(playerName);
    }

    [Command(ignoreAuthority = true)]
    private void CmdGiveControl(string playerName)
    {
        MaroonNetworkManager.Instance.ServerGrantControl(playerName);
    }
}
