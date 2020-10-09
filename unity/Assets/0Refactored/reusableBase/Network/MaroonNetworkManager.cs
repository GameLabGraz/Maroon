using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterSpawnMessage : MessageBase
{
    public Vector3 CharacterPosition;
    public Quaternion CharacterRotation;
}

public class NetworkControlMessage : MessageBase
{
    public string NewPlayerInControl;
}

public class ChangeSceneMessage : MessageBase
{
    public string SceneName;
}

public class MaroonNetworkManager : NetworkManager
{
    [HideInInspector]
    public static MaroonNetworkManager Instance = null;

    [Header("Maroon Network Manager")]
    [SerializeField] private GameObject experimentPlayer;
    [SerializeField] private GameObject controlHandlingUi;
    [SerializeField] private GameObject sceneCountdown;
    [Scene]
    [SerializeField] private List<string> networkEnabledExperiments;
    
    private ListServer _listServer;
    private MaroonNetworkDiscovery _networkDiscovery;
    private PortForwarding _upnp;
    private GameManager _gameManager;
    private DialogueManager _dialogueManager;

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

    private bool CheckSceneValid(string sceneName)
    {
        if (onlineScene.Contains(sceneName))
            return true;

        foreach (var enabledScene in networkEnabledExperiments)
        {
            if (enabledScene.Contains(sceneName))
                return true;
        }

        return false;
    }

    #region Server

    private bool _activePortMapping;
    private bool _sceneCountdownActive;
    private Dictionary<string, NetworkConnection> _connectedPlayers = new Dictionary<string, NetworkConnection>();
    private Dictionary<NetworkConnection, NetworkPlayer> _connectedPlayerObjects = new Dictionary<NetworkConnection, NetworkPlayer>();
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        MarkTakenServerNames();
        _serverName = NetworkNamingService.GetNewServerName();
        
        _networkDiscovery.AdvertiseServer();
        _upnp.SetupPortForwarding();
        NetworkServer.RegisterHandler<CharacterSpawnMessage>(OnCreateCharacter);
        NetworkServer.RegisterHandler<ChangeSceneMessage>(OnChangeSceneMessage);
        ClientInControl = null;
        _sceneCountdownActive = false;
        networkSceneName = onlineScene; // to make sure that Manager has Scene Control!
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkNamingService.FreeAllPlayerNames();
        NetworkNamingService.FreeAllServerNames();
        _activePortMapping = false;
        _connectedPlayers.Clear();
        DisplayMessage("StopHost");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (!SceneManager.GetActiveScene().name.Contains("Laboratory"))
        {
            //Connection only possible when in Laboratory
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        string disconnectedPlayerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        if (disconnectedPlayerName == null)
            return;
        if (disconnectedPlayerName == _clientInControl)
        {
            ServerSetClientInControl(_playerName);
        }
        _connectedPlayers.Remove(disconnectedPlayerName);
        NetworkNamingService.FreePlayerName(disconnectedPlayerName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        _sceneCountdownActive = false;
    }

    private void OnCreateCharacter(NetworkConnection conn, CharacterSpawnMessage message)
    {
        if (_connectedPlayerObjects.ContainsKey(conn)) //already created a player object for this connection!
            return;
        
        GameObject playerObject;
        if (SceneManager.GetActiveScene().name.Contains("Laboratory"))
        {
            playerObject = Instantiate(playerPrefab);
            playerObject.transform.position = message.CharacterPosition;
            playerObject.transform.rotation = message.CharacterRotation;
        }
        else
        {
            playerObject = Instantiate(experimentPlayer);
        }

        string playerName;
        if (_connectedPlayers.ContainsValue(conn))
        {
            playerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        }
        else
        {
            playerName = NetworkNamingService.GetNewPlayerName();
            if (playerName == "")
            {
                playerName = "Scientist " + conn.connectionId;
            }
            _connectedPlayers[playerName] = conn;
        }

        NetworkPlayer networkPlayer = playerObject.GetComponent<NetworkPlayer>();

        _connectedPlayerObjects[conn] = networkPlayer;
        NetworkServer.AddPlayerForConnection(conn, playerObject);
        networkPlayer.SetName(playerName);
        
        //Update client in control on new client
        NetworkControlMessage msg = new NetworkControlMessage
        {
            NewPlayerInControl = _clientInControl
        };
        conn.Send(msg);
    }

    public void RemovePlayerForConnection(NetworkConnection conn)
    {
        _connectedPlayerObjects.Remove(conn);
    }

    private void OnChangeSceneMessage(NetworkConnection conn, ChangeSceneMessage msg)
    {
        if(_sceneCountdownActive)
            return;
        if (conn == _connectedPlayers[_clientInControl] && CheckSceneValid(msg.SceneName))
        {
            GameObject coundownObject = Instantiate(sceneCountdown);
            coundownObject.GetComponent<SceneChangeCountdown>().SetSceneName(msg.SceneName);
            NetworkServer.Spawn(coundownObject);
            _sceneCountdownActive = true;
        }
    }

    public bool PortsMapped
    {
        get => _activePortMapping;
        set
        {
            _activePortMapping = value;
            if (!value)
            {
                DisplayMessage("PortMappingFail");
            }
            else if (!_listServer.GetListServerStatus())
            {
                DisplayMessage("ListServerFail");
            }
            else
            {
                DisplayMessage("PortMappingSuccess");
            }
        }
    }

    public void ServerSpawnControlHandlingUi()
    {
        GameObject ui = Instantiate(controlHandlingUi);
        NetworkServer.Spawn(ui);
    }
    
    public void ServerSetClientInControl(string clientInControl)
    {
        if (!NetworkServer.active)
            return;

        // Player not connected - Player in control stays in control
        if (!_connectedPlayers.ContainsKey(clientInControl))
        {
            return;
        }
        
        NetworkControlMessage msg = new NetworkControlMessage
        {
            NewPlayerInControl = clientInControl
        };

        foreach (var conn in _connectedPlayers.Values)
        {
            conn.Send(msg);
        }
    }

    private void MarkTakenServerNames()
    {
        foreach (var server in _listServer.list.Values)
        {
            NetworkNamingService.ServerNameTaken(server.title);
        }
    }

    #endregion

    #region Client

    private bool _tryClientConnect = true;
    private bool _hasLoadedSceneOnce;
    private string _playerName;
    private string _serverName;
    private string _clientInControl;

    [HideInInspector]
    public UnityEvent onGetControl;
    [HideInInspector]
    public UnityEvent onLoseControl;
    [HideInInspector] 
    public UnityEvent newClientInControlEvent;

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<NetworkControlMessage>(OnNetworkControlMessage);
        if (mode == NetworkManagerMode.ClientOnly)
        {
            _networkDiscovery.StopDiscovery();
            _tryClientConnect = true;
            _hasLoadedSceneOnce = false;
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
        else if (_hasLoadedSceneOnce)
        {
            //Disconnected from host
            DisplayMessage("ClientDisconnect");
        }
        else
        {
            DisplayMessage("ServerInExperiment");
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _playerName = null;
        ClientInControl = _playerName;
        _serverName = null;
        _networkDiscovery.StartDiscovery();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        _tryClientConnect = false;
        SendCreatePlayerMessage(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        _hasLoadedSceneOnce = true;
        base.OnClientSceneChanged(conn);
        SendCreatePlayerMessage(conn);
        if (IsInControl)
        {
            onGetControl.Invoke();
        }
        else
        {
            onLoseControl.Invoke();
        }
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
    
    private void OnNetworkControlMessage(NetworkConnection conn, NetworkControlMessage msg)
    {
        ClientInControl = msg.NewPlayerInControl;
    }

    public bool IsInControl => _clientInControl == _playerName;

    public string ClientInControl
    {
        get => _clientInControl;
        set
        {
            if (_clientInControl == value)
                return;

            bool loseControl = IsInControl;

            _clientInControl = value;

            if (loseControl)
            {
                onLoseControl.Invoke();
            }
            else if (value == _playerName)
            {
                onGetControl.Invoke();
            }
            newClientInControlEvent.Invoke();
        }
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            DisplayWelcomeMessage(value, _serverName);
        }
    }

    public string ServerName
    {
        get => _serverName;
        set => _serverName = value;
    }

    private float _lastEnterSceneTime;
    public void EnterScene(string sceneName)
    {
        if (mode == NetworkManagerMode.Offline)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (Time.time - _lastEnterSceneTime < 1) //Otherwise executed multiple times!
                return;
            _lastEnterSceneTime = Time.time;
            
            if (sceneName.Contains("Menu"))
            {
                DisplayMessage("MainMenuDenial");
                return;
            }
            if (IsInControl)
            {
                if (CheckSceneValid(sceneName))
                {
                    ChangeSceneMessage msg = new ChangeSceneMessage
                    {
                        SceneName = sceneName
                    };
                    NetworkClient.connection.Send(msg);
                }
                else
                {
                    DisplayMessage("ExperimentNotEnabled");
                }
            }
            else
            {
                DisplayMessage("ControlDenial");
            }
        }
    }

    public bool AllowNetworkPause()
    {
        if (!NetworkClient.active)
            return true;

        if (SceneManager.GetActiveScene().name.Contains("Laboratory"))
            return true;

        //In experiment while multi-user
        return false;
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

    private void DisplayWelcomeMessage(string playerName, string serverName)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        string message = LanguageManager.Instance.GetString("WelcomeMessage1") + " " + playerName +
                         "! ";
        message += LanguageManager.Instance.GetString("WelcomeMessage2") + " " + serverName +
                   "!";
        
        _dialogueManager.ShowMessage(message);
    }
}
