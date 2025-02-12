using UnityEngine;
using UnityEngine.Events;

namespace Maroon.GlobalEntities
{
    /// <summary>
    ///     WebGL data event for incoming data messages.
    /// </summary>
    public class WebGlDataEvent : UnityEvent<string> {}

    /// <summary>
    ///     Receives incoming messages from JavaScript code.
    /// </summary>
    public class WebGlReceiver : MonoBehaviour, GlobalEntity
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields
        private static WebGlReceiver _instance;

        public WebGlDataEvent OnIncomingData = new WebGlDataEvent();

        public UnityEvent OnPauseRequest = new UnityEvent();

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters
        public string MostRecentData { get; private set; }

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        /// <summary>
        ///     The WebGlReceiver instance
        /// </summary>
        public static WebGlReceiver Instance => WebGlReceiver._instance;
        MonoBehaviour GlobalEntity.Instance => Instance;

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
            if (WebGlReceiver._instance == null)
            {
                WebGlReceiver._instance = this;
            }
            else if (WebGlReceiver._instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            // Keep alive
            this.transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Data from JavaScript

        /// <summary>
        ///     Called from the JavaScript code.
        ///     unityInstance.SendMessage('WebGlReceiver', 'GetDataFromJavaScript', data);
        /// </summary>
        public void GetDataFromJavaScript(string data)
        {
            Debug.Log("Received Data: " + data);
            MostRecentData = data;
            OnIncomingData.Invoke(data);
        }

        
        /// <summary>
        /// Called from Javascript code when Escape is pressed.
        /// Useful because Input.GetKeyDown(KeyCode.Escape) might not always be caught,
        /// because browser unlocks mouse cursor when pressing Escape while cursor is locked
        /// </summary>
        public void PauseRequest()
        {
            Debug.Log("Unity received a PauseRequest from Javascript");
            OnPauseRequest?.Invoke();
        }
    }
}