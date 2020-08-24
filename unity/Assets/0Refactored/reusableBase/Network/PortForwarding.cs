using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Open.Nat;
using UnityEngine;

public class PortForwarding : MonoBehaviour
{
    private MaroonNetworkManager _networkManager;
    //public Text ipText;
    private NatDevice _foundDevice;
    private Mapping _createdMapping;
    private bool _activeMapping;

    private void Start()
    {
        _networkManager = GetComponent<MaroonNetworkManager>();
    }

    public async void SetupPortForwarding()
    {
        var discoverer = new NatDiscoverer();
        _createdMapping = new Mapping(Protocol.Tcp, 7777, 7777, "Maroon");
        
        try
        {
            var cts = new CancellationTokenSource(5000);
            var upnpDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
            await upnpDevice.CreatePortMapAsync(_createdMapping);
            Debug.Log("Port Mapping created successfully using UPnP");
            _foundDevice = upnpDevice;
            _activeMapping = true;
            _networkManager.PortsMapped();
            return;
        }
        catch(NatDeviceNotFoundException e)
        {
            Debug.Log("Open.NAT wasn't able to find a UpnP device ;(");
        }
        catch(MappingException me)
        {
            Debug.Log("Port Mapping could not be created using UPnPÜ");
        }
        
        //Upnp mapping didn't work, try PMP
        try
        {
            var cts = new CancellationTokenSource(5000);
            var pmpDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
            await pmpDevice.CreatePortMapAsync(_createdMapping);
            Debug.Log("Port Mapping created successfully using PMP");
            _foundDevice = pmpDevice;
            _activeMapping = true;
            _networkManager.PortsMapped();
        }
        catch(NatDeviceNotFoundException e)
        {
            Debug.Log("Open.NAT wasn't able to find a PMP device ;(");
        }
        catch(MappingException me)
        {
            Debug.Log("Port Mapping could not be created using PMP");
        }
    }

    public async void DeletePortMapping()
    {
        if (!_activeMapping)
            return;
        await _foundDevice.DeletePortMapAsync(_createdMapping);
        Debug.Log("Mapping deleted");
    }
}
