using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NetworkPlayer : NetworkBehaviour
{
    public GameObject firstPersonCharacter;

    private CharacterController _cc;
    private AudioSource _as;
    private FirstPersonController _fpc;
    private GameManager _gm;

    [SyncVar] private string _name;
    
    // Start is called before the first frame update
    void Start()
    {
        if (firstPersonCharacter == null) //not in Laboratory
            return;
        _cc = GetComponent<CharacterController>();
        _as = GetComponent<AudioSource>();
        _fpc = GetComponent<FirstPersonController>();
        _gm = FindObjectOfType<GameManager>();

        if (isLocalPlayer)
        {
            firstPersonCharacter.SetActive(true);
            _cc.enabled = true;
            _as.enabled = true;
            _fpc.enabled = true;
            
            _gm.RegisterNetworkPlayer(gameObject);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        if (MaroonNetworkManager.Instance.PlayerName == null && _name != null)
        {
            MaroonNetworkManager.Instance.PlayerName = _name;
        }
    }

    public void SetName(string newName)
    {
        _name = newName;
    }

    private void OnDestroy()
    {
        if (firstPersonCharacter == null) //not in Laboratory
            return;
        if (isLocalPlayer)
        {
            _gm.UnregisterNetworkPlayer();
        }
    }
}
