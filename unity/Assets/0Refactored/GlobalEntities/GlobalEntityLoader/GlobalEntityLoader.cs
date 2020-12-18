using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon{

    public class GlobalEntityLoader : MonoBehaviour
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        // Singleton instance
        private static GlobalEntityLoader instance = null;

        // Prefabs
        [SerializeField] private GameObject bootstrappingManagerPrefab;
        [SerializeField] private GameObject platformManagerPrefab;
        [SerializeField] private GameObject sceneManagerPrefab;
        
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
            
            Debug.Log(Maroon.PlatformManager.Instance);

            if(Maroon.PlatformManager.Instance == null)
            {
                Debug.Log("GlobalEntityLoader -> Instantiate PlatformManager");
                Instantiate(this.platformManagerPrefab).transform.SetParent(this.transform);
            }
            if(Maroon.SceneManager.Instance == null)
            {
                Instantiate(this.sceneManagerPrefab).transform.SetParent(this.transform);
            }
            if(Maroon.BootstrappingManager.Instance == null)
            {
                Instantiate(this.bootstrappingManagerPrefab).transform.SetParent(this.transform);
            }

            Debug.Log(Maroon.PlatformManager.Instance);
        }

    }
}