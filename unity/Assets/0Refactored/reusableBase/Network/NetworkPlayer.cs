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
    
    // Start is called before the first frame update
    void Start()
    {
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
}
