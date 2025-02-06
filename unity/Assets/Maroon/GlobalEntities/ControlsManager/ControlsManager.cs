using UnityEngine;

namespace Maroon.GlobalEntities.ControlsManager
{
    /// <summary>
    ///     Handles settings related to controls in Maroon.
    ///     e.g. Mouse sensitivity
    /// </summary>
    public class ControlsManager : MonoBehaviour, GlobalEntity
    {
        private static ControlsManager _instance = null;
        /// <summary>
        ///     The ControlsManager instance
        /// </summary>
        public static ControlsManager Instance => ControlsManager._instance;
        MonoBehaviour GlobalEntity.Instance => Instance;


        private float _mouseSensitivity = 200f;
        /// <summary>
        ///     The sensitivity of the mouse for first-person controls, e.g. when looking around in the laboratory.
        /// </summary>
        public float MouseSensitivity
        {
            get { return _mouseSensitivity; }
            set { _mouseSensitivity = value; }
        }

        /// <summary>
        ///     Called by Unity. Initializes singleton instance and DontDestroyOnLoad (stays active on new scene load).
        /// </summary>
        private void Awake()
        {
            // Singleton
            if(ControlsManager._instance == null)
            {
                ControlsManager._instance = this;
            }
            else if(ControlsManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            
            // Keep alive
            this.transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
