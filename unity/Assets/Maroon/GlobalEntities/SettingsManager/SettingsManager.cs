using System.Collections;
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
        /// The currently in-use SettingsResource
        /// </summary>
        public SettingsResource Settings { get; private set; }

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

        private IEnumerator Start()
        {
            // Wait a frame, otherwise LanguageManager will not have been initialized
            yield return null;

            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings = SaveHelper.LoadSettingsResource();
            Settings.ApplyAllSettings();
            Settings.settingsChangedEvent?.AddListener(SaveSettings);
        }

        private void SaveSettings()
        {
            SaveHelper.SaveSettingsResource(Settings);
        }
    }
}
