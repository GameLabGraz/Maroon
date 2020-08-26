using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Util;

public class NetworkConnectTrigger : MonoBehaviour
{
    [SerializeField] private NetworkScreen _screen;
    
    private MaroonNetworkManager _networkManager;

    private void Start()
    {
        _networkManager = FindObjectOfType<MaroonNetworkManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PlayerUtil.IsPlayer(other.gameObject))
            return;

        if (Input.GetKey(KeyCode.Return))
        {
            _networkManager.StartMultiUser();
            //TODO: use language manager
            _screen.DisplayMessage("Go to the menu to join a network!");
        }
    }
}
