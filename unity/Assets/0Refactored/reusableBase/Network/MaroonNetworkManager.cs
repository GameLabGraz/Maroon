using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MaroonNetworkManager : NetworkManager
{
    private ListServer _listServer;
    private MaroonNetworkDiscovery _networkDiscovery;
    private PortForwarding _upnp;

    private bool _activePortMapping;
    
    public override void Start()
    {
        base.Start();
        _listServer = GetComponent<ListServer>();
        _networkDiscovery = GetComponent<MaroonNetworkDiscovery>();
        _upnp = GetComponent<PortForwarding>();
        
        _listServer.ConnectToListServer();
        _networkDiscovery.StartDiscovery();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("Host started");
        _networkDiscovery.AdvertiseServer();
        _upnp.SetupPortForwarding();
    }

    public void PortsMapped()
    {
        //TODO: Manual Port Mapping
        _activePortMapping = true;
        _listServer.PortMappingSuccessfull();
    }
    
    public override void OnApplicationQuit()
    {
        if (_activePortMapping)
        {
            _upnp.DeletePortMapping();
        }
        base.OnApplicationQuit();
    }
}
