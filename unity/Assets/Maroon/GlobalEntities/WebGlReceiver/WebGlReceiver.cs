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

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters

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
            OnIncomingData.Invoke(data);
        }
    }
}