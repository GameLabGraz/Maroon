using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnMessage : MessageBase
{
    public Vector3 CharacterPosition;
    public Quaternion CharacterRotation;
}

public class NetworkControlMessage : MessageBase
{
    public bool InControl;
}

public class ChangeSceneMessage : MessageBase
{
    public string SceneName;
}

public class MaroonNetworkManager : NetworkManager
{
    [HideInInspector]
    public static MaroonNetworkManager Instance = null;

    public GameObject preNetworkSyncVars;
    
    private ListServer _listServer;
    private MaroonNetworkDiscovery _networkDiscovery;
    private PortForwarding _upnp;
    private GameManager _gameManager;
    private DialogueManager _dialogueManager;
    private NetworkSyncVariables _syncVariables;

    private bool _isStarted;

    public override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        _listServer = GetComponent<ListServer>();
        _networkDiscovery = GetComponent<MaroonNetworkDiscovery>();
        _upnp = GetComponent<PortForwarding>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void StartMultiUser()
    {
        if(_isStarted)
            return;
        _listServer.ConnectToListServer();
        _networkDiscovery.StartDiscovery();
        _isStarted = true;
    }

    public bool IsActive()
    {
        return _isStarted;
    }

    #region Server

    private bool _activePortMapping;
    private string _clientInControl;
    private Dictionary<string, NetworkConnection> _connectedPlayers = new Dictionary<string, NetworkConnection>();
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _networkDiscovery.AdvertiseServer();
        _upnp.SetupPortForwarding();
        NetworkServer.RegisterHandler<CharacterSpawnMessage>(OnCreateCharacter);
        NetworkServer.RegisterHandler<ChangeSceneMessage>(OnChangeSceneMessage);
        _isInControl = true;
        SpawnSyncVars();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        string disconnectedPlayerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        if (disconnectedPlayerName == _clientInControl)
        {
            //Cannot use TakeControl because client already disconnected
            _isInControl = true;
            _clientInControl = _playerName;
        }
        _connectedPlayers.Remove(disconnectedPlayerName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        //For some strange reason, ClientRPCs and SyncVars are not updated when this is called immediately.
        //Therefore we wait a second until we spawn it
        StartCoroutine(SpawnNetworkSyncVarsAfterSeconds(1));
    }
    
    private IEnumerator SpawnNetworkSyncVarsAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SpawnSyncVars();
    }

    private void OnCreateCharacter(NetworkConnection conn, CharacterSpawnMessage message)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        playerObject.transform.position = message.CharacterPosition;
        playerObject.transform.rotation = message.CharacterRotation;
        
        //TODO: Naming!!
        string playerName;
        if (_connectedPlayers.ContainsValue(conn))
        {
            playerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        }
        else
        {
            playerName = "Player " + conn.connectionId;
            _connectedPlayers[playerName] = conn;
        }

        playerObject.GetComponent<NetworkPlayer>().SetName(playerName);

        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }

    private void OnChangeSceneMessage(NetworkConnection conn, ChangeSceneMessage msg)
    {
        if (conn == _connectedPlayers[GetClientInControl()])
        {
            ServerChangeScene(msg.SceneName);
        }
    }
    
    public void PortsMapped()
    {
        //TODO: Manual Port Mapping
        _activePortMapping = true;
        _listServer.PortMappingSuccessfull();
    }

    public string GetClientInControl()
    {
        if (_clientInControl == null)
            _clientInControl = _playerName;
        
        return _clientInControl;
    }

    public List<string> GetAllPlayerNames()
    {
        return _connectedPlayers.Keys.ToList();
    }

    public void ServerGrantControl(string newClientInControl)
    {
        //Tell client currently in control he is not
        NetworkControlMessage msg = new NetworkControlMessage
        {
            InControl = false
        };
        _connectedPlayers[GetClientInControl()].Send(msg);
        
        //Tell new client he is in control
        msg.InControl = true;
        _connectedPlayers[newClientInControl].Send(msg);
        _clientInControl = newClientInControl;
    }

    public void TakeControl()
    {
        if (!(mode == NetworkManagerMode.Host))
            return;
        ServerGrantControl(_playerName);
    }

    private void SpawnSyncVars()
    {
        GameObject syncVars = Instantiate(preNetworkSyncVars);
        NetworkServer.Spawn(syncVars);
    }

    #endregion

    #region Client

    private bool _tryClientConnect = true;
    private bool _isInControl;
    private string _playerName;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<NetworkControlMessage>(OnNetworkControlMessage);
        if (mode == NetworkManagerMode.ClientOnly)
        {
            _networkDiscovery.StopDiscovery();
            _tryClientConnect = true;
            _isInControl = false;
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        if (_tryClientConnect)
        {
            //Connection attempt failed!
            _tryClientConnect = false;
            DisplayMessage("ClientConnectFail");
        }
        else
        {
            //Disconnected from host
            DisplayMessage("ClientDisconnect");
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _networkDiscovery.StartDiscovery();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        _tryClientConnect = false;
        
        if(SceneManager.GetActiveScene().name.Contains("Laboratory"))
            SendCreatePlayerMessage(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        
        if(SceneManager.GetActiveScene().name.Contains("Laboratory"))
            SendCreatePlayerMessage(conn);
    }

    private void SendCreatePlayerMessage(NetworkConnection conn)
    {
        // you can send the message here, or wherever else you want
        CharacterSpawnMessage characterMessage = new CharacterSpawnMessage
        {
            CharacterPosition = _gameManager.GetPlayerPosition(),
            CharacterRotation = _gameManager.GetPlayerRotation()
        };

        conn.Send(characterMessage);
    }

    public bool IsInControl => _isInControl;

    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    private void OnNetworkControlMessage(NetworkConnection conn, NetworkControlMessage msg)
    {
        _isInControl = msg.InControl;
        //TODO: Here take care of what to do with control?
    }
    
    public void EnterScene(string sceneName)
    {
        if (mode == NetworkManagerMode.Offline)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (_isInControl)
            {
                ChangeSceneMessage msg = new ChangeSceneMessage
                {
                    SceneName =  sceneName
                };
                NetworkClient.connection.Send(msg);
            }
            else
            {
                //TODO: This only works in Lab!
                DisplayMessage("ChangeSceneDenial");
            }
        }
    }

    #endregion

    public override void OnApplicationQuit()
    {
        if (_activePortMapping)
        {
            _upnp.DeletePortMapping();
        }
        base.OnApplicationQuit();
    }
    
    private void DisplayMessage(string messageKey)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        var message = LanguageManager.Instance.GetString(messageKey);
        _dialogueManager.ShowMessage(message);
    }
}
