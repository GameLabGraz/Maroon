using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon
{
    /// <summary>
    ///     Handles the creation and instantiation of global entities.
    ///
    ///     TODO: The input should just be an array of globalEntities, and they should be instantiated in a loop. But
    ///           this works for now.
    /// </summary>
    public class GlobalEntityLoader : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        // Singleton instance
        private static GlobalEntityLoader instance = null;

        // Prefabs
        [SerializeField] private GameObject _bootstrappingManagerPrefab = null;
        [SerializeField] private GameObject _platformManagerPrefab;
        [SerializeField] private GameObject _sceneManagerPrefab;
        [SerializeField] private GameObject _soundManagerPrefab;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static GlobalEntityLoader Instance
        {
            get { return GlobalEntityLoader.instance; }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        private void Awake()
        {
            // Singleton
            if(GlobalEntityLoader.instance == null)
            {
                GlobalEntityLoader.instance = this;
            }
            else if(GlobalEntityLoader.instance != this)
            {
                DestroyImmediate(this.gameObject);
            }

            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // Create instances
            if(Maroon.PlatformManager.Instance == null)
            {
                Instantiate(this._platformManagerPrefab).transform.SetParent(this.transform);
            }
            if(Maroon.SceneManager.Instance == null)
            {
                Instantiate(this._sceneManagerPrefab).transform.SetParent(this.transform);
            }
            if(Maroon.BootstrappingManager.Instance == null)
            {
                Instantiate(this._bootstrappingManagerPrefab).transform.SetParent(this.transform);
            }
            if(Maroon.SoundManager.Instance == null)
            {
                Instantiate(this._soundManagerPrefab).transform.SetParent(this.transform);
            }
        }
    }
}