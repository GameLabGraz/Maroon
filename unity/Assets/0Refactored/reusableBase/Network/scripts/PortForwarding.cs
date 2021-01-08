using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GEAR.Localization;
using Maroon.UI;
using Open.Nat;
using UnityEngine;

public class PortForwarding : MonoBehaviour
{
    //public Text ipText;
    private NatDevice _foundDevice;
    private Mapping _createdMapping;
    private bool _activeMapping;
    
    private DialogueManager _dialogueManager;

    public async void SetupPortForwarding()
    {
        var discoverer = new NatDiscoverer();
        _createdMapping = new Mapping(Protocol.Tcp, 7777, 7777, "Maroon");
        
        try
        {
            var cts = new CancellationTokenSource(3000);
            var upnpDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
            await upnpDevice.CreatePortMapAsync(_createdMapping);
            //Debug.Log("Port Mapping created successfully using UPnP");
            _foundDevice = upnpDevice;
            _activeMapping = true;
            MaroonNetworkManager.Instance.PortsMapped = true;
            return;
        }
        catch(NatDeviceNotFoundException e)
        {
            // Doesn't matter, try PMP
        }
        catch(MappingException me)
        {
            // Doesn't matter, try PMP
        }
        
        //Upnp mapping didn't work, try PMP
        try
        {
            var cts = new CancellationTokenSource(3000);
            var pmpDevice = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
            await pmpDevice.CreatePortMapAsync(_createdMapping);
            //Debug.Log("Port Mapping created successfully using PMP");
            _foundDevice = pmpDevice;
            _activeMapping = true;
            MaroonNetworkManager.Instance.PortsMapped = true;
        }
        catch(NatDeviceNotFoundException e)
        {
            MaroonNetworkManager.Instance.PortsMapped = false;
        }
        catch(MappingException me)
        {
            MaroonNetworkManager.Instance.PortsMapped = false;
        }
    }

    public async void DeletePortMapping()
    {
        if (!_activeMapping)
            return;
        await _foundDevice.DeletePortMapAsync(_createdMapping);
        //Debug.Log("Mapping deleted");
    }
}
