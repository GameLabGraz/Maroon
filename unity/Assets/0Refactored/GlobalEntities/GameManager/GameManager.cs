using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

namespace Maroon
{

    //This manager will be created one time at the beginning and stays troughout all scenes to manage 
    //the game and save settings etc. Since the game doesn't have a load/save function, we don't need Prefabs
    public class GameManager : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields
        private static GameManager _instance = null;

        [SerializeField]
        private GameObject _player;
        
        private GameObject _offlinePlayer;

        private static Vector3 _playerPosition;
        private static Quaternion _playerRotation;

        public AudioSource menuSound; //sound when player goes to menu

        public bool LabLoaded { get; private set; }

        private string _version;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        /// <summary>
        ///     The GameManager instance
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                return GameManager._instance;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        // -------------------------------------------------------------------------------------------------------------
        // Initialization

        /// <summary>
        ///     Called by Unity. Initializes singleton instance and DontDestroyOnLoad (stays active on new scene load).
        /// </summary>
        private void Awake()
        {
            // Singleton
            if(GameManager._instance == null)
            {
                GameManager._instance = this;
            }
            else if(GameManager._instance != this)
            {
                // ################
                // ----------------
                // TODO: This should not be done here, this is a very ugly hack and should be solved by actually only having
                // one GameManager at all times, not copying stuff from a temporary game manager to another one and then 
                // silently destroying the duplicate game manager
                if (_player != null && Maroon.SceneManager.Instance.ActiveSceneName.Contains("Laboratory"))
                {
                    /*
                        TODO: This needs to be done in a Player script, because the lab looks different every time now
                    _player.transform.position = _playerPosition;
                    _player.transform.rotation = _playerRotation;
                    */
                }

                Instance._player = _player;
                Instance.LabLoaded = true;
                // ----------------
                // ################

                // Only this should be done here
                Destroy(this.gameObject);
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);


            // Version
    #if UNITY_EDITOR
            _version = DateTime.UtcNow.Date.ToString("yyyyMMdd");
    #else
            _version = Application.version;
    #endif

            // Player
            if (!_player)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }
        }

        private void Update()
        {
            // TODO: This should be done by the Player OR by the Laboratory
            if(_player != null && Maroon.SceneManager.Instance.ActiveSceneName.Contains("Laboratory"))
            {
                _playerPosition = _player.transform.position;
                _playerRotation = _player.transform.rotation;
            }
        }


        // #################################################################################################################
        // #################################################################################################################
        // Things below this line should not be done in the GameManager

        // THIS IS A WORKAROUND, THERE SHOULD BE ONE (or multiple, if multiplayer) PLAYERS. The player should not be
        // destroyed and created on each scene load.
        public void searchForPlayer()
        {
            // Search for: PlayerPC in laboratory 
        }


        // #################################################################################################################
        // MOVE TO: Main Menu

        public void OnGUI()
        {
            // show build version on lower right corner
            GUI.Label(new Rect(10, Screen.height - 20f, 300f, 200f), $"build {_version}", new GUIStyle
            {
                fontSize = 14, fontStyle = FontStyle.Bold, normal = { textColor = Color.white }
            });
        }

        // #################################################################################################################
        // MOVE TO: NetworkManager or PlayerManager
        public void RegisterNetworkPlayer(GameObject newPlayer)
        {
            _offlinePlayer = _player;
            _player.SetActive(false);
            _player = newPlayer;
        }
        
        public void UnregisterNetworkPlayer()
        {
            if (_offlinePlayer == null)
            {
                _player = null;
                return;
            }

            if (_player != null)
            {
                _playerPosition = _player.transform.position;
                _playerRotation = _player.transform.rotation;
            }
            _offlinePlayer.SetActive(true);
            _offlinePlayer.transform.position = _playerPosition;
            //cannot set _offlinePlayer.transform.rotation = _playerRotation; because overruled by First Person Controller
            _offlinePlayer.GetComponent<FirstPersonController>().SetPlayerRotation(_playerRotation);
            _player = _offlinePlayer;
        }

        // #################################################################################################################
        // MOVE TO: Player or PlayerManager
        public Vector3 GetPlayerPosition()
        {
            return _playerPosition;
        }

        public Quaternion GetPlayerRotation()
        {
            return _playerRotation;
        }
    }
}