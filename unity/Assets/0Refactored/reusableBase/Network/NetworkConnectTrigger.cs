using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Util;

public class NetworkConnectTrigger : MonoBehaviour
{
    [SerializeField] private NetworkScreen _screen;

    private void OnTriggerStay(Collider other)
    {
        if (!PlayerUtil.IsPlayer(other.gameObject))
            return;

        if (Input.GetKey(KeyCode.Return))
        {
            MaroonNetworkManager.Instance.StartMultiUser();
            //TODO: use language manager
            _screen.DisplayMessage("Go to the menu to join a network!");
        }
    }
}
