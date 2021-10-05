using System.Collections.Generic;
using UnityEngine;

namespace Maroon.GlobalEntities
{
    public interface GlobalEntity
    {
        MonoBehaviour Instance { get; }
    }

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
        private static GlobalEntityLoader _instance = null;

        // Prefabs
        [SerializeField] private List<GameObject> _globalEntities  = new List<GameObject>();



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public static GlobalEntityLoader Instance => GlobalEntityLoader._instance;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Methods

        private void Awake()
        {
            // ---------------------------------------------------------------------------------------------------------
            // Singleton
            if(GlobalEntityLoader._instance == null)
            {
                GlobalEntityLoader._instance = this;
            }
            else if(GlobalEntityLoader._instance != this)
            {                
                DestroyImmediate(this.gameObject);
                return;
            }

            // ---------------------------------------------------------------------------------------------------------
            // Keep alive
            DontDestroyOnLoad(this.gameObject);

            // ---------------------------------------------------------------------------------------------------------
            // Create instances
            foreach (var globalEntityPrefab in _globalEntities)
            {
                var globalEntity = globalEntityPrefab.GetComponent<GlobalEntity>();
                if (globalEntity == null || globalEntity.Instance != null) continue;

                var clone = Instantiate(globalEntityPrefab, transform);
                clone.name = globalEntityPrefab.name;
            }
        }
    }
}