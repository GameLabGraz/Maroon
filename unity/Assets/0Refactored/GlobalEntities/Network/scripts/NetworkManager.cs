using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Maroon
{
    public class NetworkManager : Mirror.NetworkManager
    {
        private static Maroon.NetworkManager _instance = null;

        [Header("Maroon Network Manager")]
        [SerializeField] private GameObject experimentPlayer;
        [SerializeField] private GameObject controlHandlingUi;
        [SerializeField] private GameObject passwordUi;
        [SerializeField] private GameObject sceneCountdown;
        [Scene]
        [SerializeField] private List<string> networkEnabledExperiments;
        
        private ListServer _listServer;
        private MaroonNetworkDiscovery _networkDiscovery;
        private PortForwarding _upnp;
        private Maroon.GameManager _gameManager;
        private DialogueManager _dialogueManager;

        private bool _isStarted;
        private ConnectionState _connectionStatus;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        /// <summary>
        ///     The NetworkManager instance
        /// </summary>
        public static Maroon.NetworkManager Instance
        {
            get
            {
                return _instance;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        // -------------------------------------------------------------------------------------------------------------
        // Initialization
        public override void Awake()
        {
            // Singleton
            if(Maroon.NetworkManager._instance == null)
            {
                Maroon.NetworkManager._instance = this;
            }
            else if(Maroon.NetworkManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            _listServer = GetComponent<ListServer>();
            _networkDiscovery = GetComponent<MaroonNetworkDiscovery>();
            _upnp = GetComponent<PortForwarding>();
            _gameManager = FindObjectOfType<Maroon.GameManager>();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += DisplayServerInExperimentMessage;
        }

        public void StartMultiUser()
        {
            if(_isStarted)
                return;
            _listServer.ConnectToListServer();
    #if !UNITY_WEBGL
            _networkDiscovery.StartDiscovery();
    #endif
            _isStarted = true;
        }

        public bool IsActive()
        {
            return _isStarted;
        }

        public ConnectionState ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus == value)
                    return;
                
                _connectionStatus = value; 
                DisplayConnectionStatusMessage(value);
            }
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
        private bool _alreadyServerRunning;
        private Dictionary<string, NetworkConnection> _connectedPlayers = new Dictionary<string, NetworkConnection>();
        private Dictionary<NetworkConnection, NetworkPlayer> _connectedPlayerObjects = new Dictionary<NetworkConnection, NetworkPlayer>();
        private bool _passwordProtected;
        private string _serverPassword;
        private List<NetworkConnection> _authenticatedPlayers = new List<NetworkConnection>();

        public void StartHostWithPassword()
        {
            GameObject pwUi = Instantiate(passwordUi) as GameObject;
            pwUi.GetComponent<PasswordUI>().isHost = true;
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            
            MarkTakenServerNames();
            _serverName = NetworkNamingService.GetNewServerName();

            try
            {
                _networkDiscovery.AdvertiseServer();
            }
            catch (SocketException)
            {
                DisplayMessageByKey("AlreadyServerRunning");
                _alreadyServerRunning = true;
            }

            _upnp.SetupPortForwarding();
            NetworkServer.RegisterHandler<CharacterSpawnMessage>(OnCreateCharacter);
            NetworkServer.RegisterHandler<ChangeSceneMessage>(OnChangeSceneMessage);
            NetworkServer.RegisterHandler<ConnectMessage>(OnConnectMessage);
            ClientInControl = null;
            networkSceneName = onlineScene; // to make sure that Manager has Scene Control!
        }

        private void OnConnectMessage(NetworkConnection conn, ConnectMessage connMsg)
        {
            if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Laboratory"))
            {
                ServerAskPlayerToLeave(conn, LeaveReason.InExperiment);
            }
            else if (_passwordProtected && connMsg.password != _serverPassword)
            {
                ServerAskPlayerToLeave(conn, LeaveReason.WrongPw);
            }
            else
            {
                CharacterSpawnMessage charMsg = new CharacterSpawnMessage();
                conn.Send(charMsg);
                _authenticatedPlayers.Add(conn);
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            NetworkNamingService.FreeAllPlayerNames();
            NetworkNamingService.FreeAllServerNames();
            _activePortMapping = false;
            _connectedPlayers.Clear();
            
            if (!_alreadyServerRunning)
            {
                DisplayMessageByKey("StopHost");
            }
            _alreadyServerRunning = false;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            if (_authenticatedPlayers.Contains(conn))
                _authenticatedPlayers.Remove(conn);
            
            string disconnectedPlayerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
            if (disconnectedPlayerName == null)
                return;
            if (disconnectedPlayerName == _clientInControl)
            {
                ServerSetClientInControl(_playerName);
            }
            _connectedPlayers.Remove(disconnectedPlayerName);
            NetworkNamingService.FreePlayerName(disconnectedPlayerName);

            if (disconnectedPlayerName != _playerName)
            {
                string leaveMsg = disconnectedPlayerName + " " + LanguageManager.Instance.GetString("ClientLeave");
                DisplayMessage(leaveMsg);
            }
        }

        private void OnCreateCharacter(NetworkConnection conn, CharacterSpawnMessage message)
        {
            if (!_authenticatedPlayers.Contains(conn))
            {
                conn.Disconnect();
                return;
            }
            
            if (_connectedPlayerObjects.ContainsKey(conn)) //already created a player object for this connection!
                return;
            
            GameObject playerObject;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Laboratory"))
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

                if (_playerName != null) //Don't display for Host
                {
                    string joinMsg = playerName + " " + LanguageManager.Instance.GetString("ClientJoin");
                    DisplayMessage(joinMsg);
                }
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
            }
        }

        public bool SceneCountdownActive
        {
            get => _sceneCountdownActive;
            set => _sceneCountdownActive = value;
        }

        public string Password
        {
            set
            {
                _serverPassword = value;
                ClientPassword = value;
                _passwordProtected = _serverPassword != "";
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
                    DisplayMessageByKey("PortMappingFail");
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

        public List<string> ServerGetPlayerNames()
        {
            return _connectedPlayers.Keys.ToList();
        }

        public NetworkConnection GetConnectionByName(string playerName)
        {
            return _connectedPlayers[playerName];
        }

        public void KickPlayer(string playerName)
        {
            if (!_connectedPlayers.ContainsKey(playerName))
                return;
            
            ServerAskPlayerToLeave(_connectedPlayers[playerName], LeaveReason.Kicked);
        }

        private void ServerAskPlayerToLeave(NetworkConnection conn, LeaveReason reason)
        {
            LeaveMessage msg = new LeaveMessage
            {
                Reason = reason
            };
            conn.Send(msg);
            StartCoroutine(DisconnectAfterWait(conn));
        }

        private IEnumerator DisconnectAfterWait(NetworkConnection conn)
        {
            yield return new WaitForSecondsRealtime(5);
            try
            {
                conn.Disconnect();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                //Already Disconnected
            }
        }

        #endregion

        #region Client

        private string _playerName;
        private string _serverName;
        private string _clientInControl;
        private LeaveReason _leaveReason;
        private string _clientPassword;
        private bool _serverInExperiment;
        private string _sceneBeforeJoin = null;

        [HideInInspector]
        public UnityEvent onGetControl;
        [HideInInspector]
        public UnityEvent onLoseControl;
        [HideInInspector] 
        public UnityEvent newClientInControlEvent;
        
        public void StartClientWithPassword()
        {
            GameObject pwUi = Instantiate(passwordUi) as GameObject;
            pwUi.GetComponent<PasswordUI>().isHost = false;
        }

        public override void OnStartClient()
        {
            _sceneBeforeJoin = Maroon.SceneManager.Instance.ActiveSceneName;

            base.OnStartClient();
            
            if(_alreadyServerRunning)
                StopHost();
            else
            {
                NetworkClient.RegisterHandler<NetworkControlMessage>(OnNetworkControlMessage);
                NetworkClient.RegisterHandler<CharacterSpawnMessage>(OnCharacterSpawnMessage);
                NetworkClient.RegisterHandler<LeaveMessage>(OnLeaveMessage);
            }

            if (mode == NetworkManagerMode.ClientOnly)
            {
    #if !UNITY_WEBGL
                _networkDiscovery.StopDiscovery();
    #endif
                _leaveReason = LeaveReason.NoConnection;
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            _playerName = null;
            ClientInControl = _playerName;
            _serverName = null;
    #if !UNITY_WEBGL
            _networkDiscovery.StartDiscovery();
    #endif

            if (mode == NetworkManagerMode.Host)
                return;

            string leaveMessageKey;

            switch (_leaveReason)
            {
                case LeaveReason.Usual:
                    leaveMessageKey = "UsualClientLeave";
                    break;
                case LeaveReason.InExperiment:
                    leaveMessageKey = "ServerInExperiment";
                    _serverInExperiment = true;
                    Maroon.CustomSceneAsset asset = Maroon.SceneManager.Instance.GetSceneAssetBySceneName(_sceneBeforeJoin);
                    Maroon.SceneManager.Instance.LoadSceneSilentlyLocalOnlyExecuteForce(asset);
                    break;
                case LeaveReason.Kicked:
                    leaveMessageKey = "ClientKicked";
                    break;
                case LeaveReason.WrongPw:
                    leaveMessageKey = "WrongPassword";
                    break;
                case LeaveReason.NoConnection:
                    leaveMessageKey = "ClientConnectFail";
                    break;
                default:
                    leaveMessageKey = "ClientDisconnect";
                    break;
            }
            
            DisplayMessageByKey(leaveMessageKey);
        }

        private void DisplayServerInExperimentMessage(Scene scene, LoadSceneMode lsm)
        {
            if (_serverInExperiment && scene.name.Contains("Laboratory"))
            {
                _serverInExperiment = false;
                DisplayMessageByKey("ServerInExperiment");
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            _leaveReason = LeaveReason.Disconnect;
            ConnectMessage msg = new ConnectMessage
            {
                password = _clientPassword
            };
            conn.Send(msg);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
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

            Maroon.CustomSceneAsset asset = Maroon.SceneManager.Instance.GetSceneAssetBySceneName(Maroon.SceneManager.Instance.ActiveSceneName);
            Maroon.SceneManager.Instance.AddToSceneHistory(asset);
        }

        private void OnCharacterSpawnMessage(NetworkConnection conn, CharacterSpawnMessage msg)
        {
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
        
        private void OnNetworkControlMessage(NetworkConnection conn, NetworkControlMessage msg)
        {
            ClientInControl = msg.NewPlayerInControl;
        }

        private void OnLeaveMessage(NetworkConnection conn, LeaveMessage msg)
        {
            _leaveReason = msg.Reason;
            StopClient();
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

        public string ClientPassword
        {
            set => _clientPassword = value;
        }

        private float _lastEnterSceneTime;



        // Starts countdown for scene change
        public void EnterScene(string sceneName)
        {
            if(mode == NetworkManagerMode.Offline)
            {
                return;
            }
            
            if (Time.time - _lastEnterSceneTime < 1) //Otherwise executed multiple times!
                return;
            _lastEnterSceneTime = Time.time;
            
            if (sceneName.Contains("Menu"))
            {
                DisplayMessageByKey("MainMenuDenial");
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
                    DisplayMessageByKey("ExperimentNotEnabled");
                }
            }
            else
            {
                DisplayMessageByKey("ControlDenial");
            }
            
        }

        public bool AllowNetworkPause()
        {
            if (!NetworkClient.active)
                return true;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Laboratory"))
                return true;

            //In experiment while multi-user
            return false;
        }
        
        public void StopClientRegularly()
        {
            _leaveReason = LeaveReason.Usual;
            StopClient();
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
        
        private void DisplayMessageByKey(string messageKey)
        {
            var message = LanguageManager.Instance.GetString(messageKey);
            DisplayMessage(message);
        }

        private void DisplayWelcomeMessage(string playerName, string serverName)
        {
            string message = LanguageManager.Instance.GetString("WelcomeMessage1") + " " + playerName +
                            "! ";
            message += LanguageManager.Instance.GetString("WelcomeMessage2") + " " + serverName +
                    "!";
            
            DisplayMessage(message);
        }

        private void DisplayMessage(string text)
        {
            if (_dialogueManager == null)
                _dialogueManager = FindObjectOfType<DialogueManager>();

            if (_dialogueManager == null)
                return;

            _dialogueManager.ShowMessage(text);
        }

        private void DisplayConnectionStatusMessage(ConnectionState status)
        {
            switch (status)
            {
                case ConnectionState.Offline:
                    DisplayMessageByKey("Offline");
                    break;
                case ConnectionState.OfflineNoConnectionToListServer:
                    DisplayMessageByKey("ListServerFailOffline");
                    break;
                case ConnectionState.ClientOnline:
                    break;
                case ConnectionState.HostOnline:
                    DisplayMessageByKey("PortMappingSuccess");
                    break;
                case ConnectionState.HostOnlineNoConnectionToListServer:
                    DisplayMessageByKey("ListServerFailHost");
                    break;
                case ConnectionState.HostOnlinePortsNotMapped:
                    //Displayed only on fail
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}

public class ConnectMessage : MessageBase
{
    public string password;
}

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

public enum LeaveReason
{
    Disconnect,
    Usual,
    InExperiment,
    Kicked,
    WrongPw,
    NoConnection
}

public class LeaveMessage : MessageBase
{
    public LeaveReason Reason;
}

public enum ConnectionState
{
    Offline,
    OfflineNoConnectionToListServer,
    ClientOnline,
    HostOnline,
    HostOnlineNoConnectionToListServer,
    HostOnlinePortsNotMapped
}
