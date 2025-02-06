using UnityEngine;

namespace Maroon.GlobalEntities.SettingsManager
{
    /// <summary>
    ///     Responsible for storing settings persistently on the Computer
    ///     (i.e. Saves audio volume levels, selected language, and mouse sensitivity)
    /// </summary>
    public class SettingsManager : MonoBehaviour, GlobalEntity
    {
        private static SettingsManager _instance = null;
        /// <summary>
        ///     The SettingsManager instance
        /// </summary>
        public static SettingsManager Instance => SettingsManager._instance;
        MonoBehaviour GlobalEntity.Instance => Instance;

        /// <summary>
        ///     Called by Unity. Initializes singleton instance and DontDestroyOnLoad (stays active on new scene load).
        /// </summary>
        private void Awake()
        {
            // Singleton
            if(SettingsManager._instance == null)
            {
                SettingsManager._instance = this;
            }
            else if(SettingsManager._instance != this)
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
