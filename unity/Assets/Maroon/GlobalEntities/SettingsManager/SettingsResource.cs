using GEAR.Localization;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.GlobalEntities.SettingsManager
{
    public class SettingsResource
    {
        [NonSerialized] public UnityEvent settingsChangedEvent = new UnityEvent();

        #region Settings

        [SerializeField] private float musicVolume = 1.0f;
        public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                musicVolume = value;
                SoundManager.Instance.MusicVolume = value;
                settingsChangedEvent?.Invoke();
            }
        }

        [SerializeField] private float soundEffectVolume = 1.0f;
        public float SoundEffectVolume
        {
            get { return soundEffectVolume; }
            set
            {
                soundEffectVolume = value;
                SoundManager.Instance.SoundEffectVolume = value;
                settingsChangedEvent?.Invoke();
            }
        }

        [SerializeField] private SystemLanguage language = SystemLanguage.English;
        public SystemLanguage Language
        {
            get { return language; }
            set
            {
                language = value;
                LanguageManager.Instance.CurrentLanguage = language;
                settingsChangedEvent?.Invoke();
            }
        }

        [SerializeField] private float mouseSensitivity = 200.0f;
        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
            set
            {
                mouseSensitivity = value;
                // TODO set mouse sensitivity
                settingsChangedEvent?.Invoke();
            }
        }
        #endregion
    }
}