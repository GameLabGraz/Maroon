using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject firstPersonCharacter;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject colouredBody;
    [SerializeField] private TextMeshProUGUI playerNameText;

    private CharacterController _cc;
    private AudioSource _as;
    private FirstPersonController _fpc;
    private GameManager _gm;

    private Color _noControlColor = new Color(0.5f, 0, 0);
    private Color _inControlColor = new Color(0, 0.5f, 0);

    [SyncVar(hook = "OnChangeName")] private string _name;
    
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

            body.SetActive(false);
            
            _gm.RegisterNetworkPlayer(gameObject);
        }
        else
        {
            InvokeRepeating(nameof(UpdateControlColor), 0, 0.5f);
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

    private void OnChangeName(string oldName, string newName)
    {
        if (isLocalPlayer)
        {
            MaroonNetworkManager.Instance.PlayerName = newName;
        }
        else if(firstPersonCharacter != null) //in Laboratory
        {
            playerNameText.text = newName;
        }
    }

    private void UpdateControlColor()
    {
        if (_name == NetworkSyncVariables.Instance.ClientInControl)
        {
            SetColor(_inControlColor);
        }
        else
        {
            SetColor(_noControlColor);
        }
    }

    private void SetColor(Color color)
    {
        foreach (var rend in colouredBody.GetComponentsInChildren<Renderer>())
        {
            rend.material.color = color;
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
