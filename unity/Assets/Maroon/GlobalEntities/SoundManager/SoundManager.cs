using UnityEngine;

namespace Maroon.GlobalEntities
{
    /// <summary>
    ///     Handles tasks related to sound in Maroon. It keeps settings about audio levels, can play audio effects that
    ///     are common in Maroon (e.g. alerts or clicks) and plays background music.
    /// </summary>
    public class SoundManager : MonoBehaviour, GlobalEntity
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        private static SoundManager _instance = null;

        // -------------------------------------------------------------------------------------------------------------
        // Sources

        // For sound effects
        [SerializeField] private AudioSource _soundEffectSource = null;

        // For music
        [SerializeField] private AudioSource _musicSource = null;

        // -------------------------------------------------------------------------------------------------------------
        // Common Sounds

        /// <summary>
        ///     Sound to be played for clicks or default actions in the user interface. E.g. clicking on a button.
        /// </summary>
        [SerializeField] private AudioClip _uiPrimary = null;

        /// <summary>
        ///     Sound to be played for information actions. E.g. an alert about a system state change.
        /// </summary>
        [SerializeField] private AudioClip _uiInfo = null;


        /// <summary>
        ///     Sound to be played for successful actions. E.g. an alert about a succesful connection to a server.
        /// </summary>
        [SerializeField] private AudioClip _uiSuccess = null;

        /// <summary>
        ///     Sound to be played for actions. E.g. an alert about a disconnect from a server.
        /// </summary>
        [SerializeField] private AudioClip _uiWarning = null;

        /// <summary>
        ///     Sound to be played for failed actions. E.g. an alert about a failed attempt to load a resource.
        /// </summary>
        [SerializeField] private AudioClip _uiDanger = null;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties, Getters and Setters

        // -------------------------------------------------------------------------------------------------------------
        // Singleton

        /// <summary>
        ///     The GameManager instance
        /// </summary>
        public static SoundManager Instance => SoundManager._instance;
        MonoBehaviour GlobalEntity.Instance => Instance;

        // -------------------------------------------------------------------------------------------------------------
        // Audio sources
        
        /// <summary>
        ///     Getter for Sound Effect AudioSource component
        /// </summary>
        public AudioSource soundEffectSource => _soundEffectSource;

        // -------------------------------------------------------------------------------------------------------------
        // Volume

        /// <summary>
        ///     The volume of the music audio source. A float from 0.0 to 1.0.
        /// </summary>
        public float MusicVolume
        {
            get { return _musicSource ? _musicSource.volume : 0f; }
            set { 
                if(_musicSource)
                    _musicSource.volume = Mathf.Clamp(value, 0f, 1f); 
            }
        }

        /// <summary>
        ///     The volume of the sound effect audio source. A float from 0.0 to 1.0.
        /// </summary>
        public float SoundEffectVolume
        {
            get { return _soundEffectSource ? _soundEffectSource.volume : 0f; }
            set
            {
                if(_soundEffectSource)
                    _soundEffectSource.volume = Mathf.Clamp(value, 0f, 1f);
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
            if(SoundManager._instance == null)
            {
                SoundManager._instance = this;
            }
            else if(SoundManager._instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            // Keep alive
            this.transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Play Effects

        public void PlaySoundEffect(AudioClip clip)
        {
            _soundEffectSource.clip = clip;
            _soundEffectSource.Play();
        }
        
        //TODO: Methods for default sounds
    }
}
