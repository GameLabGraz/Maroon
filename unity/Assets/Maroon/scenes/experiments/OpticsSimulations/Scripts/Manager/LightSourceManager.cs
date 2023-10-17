using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightSource;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class LightSourceManager : MonoBehaviour
    {
        public static LightSourceManager Instance;

        [SerializeField] private List<LightSource> _lightSources; // TODO dont serialize after testing is finished
        [SerializeField] private GameObject tableObjects;
        
        [Header("Prefabs: Light Sources")] 
        [SerializeField] private LightSource singleLaser;

        public List<LightSource> lightSources => _lightSources;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed LightSourceManager");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Spawn 2 lasers for testing
            AddLightSource(singleLaser, new Vector3(1.74f, 0, 0.5f));
            AddLightSource(singleLaser, new Vector3(1.69f, 0, 0.32f));
        }

        public void AddLightSource(LightSource ls, Vector3 pos)
        {
            var lsClone = Instantiate(ls, tableObjects.transform);
            lsClone.transform.localPosition = pos;
            _lightSources.Add(lsClone);
        }

        public void RemoveLightSource(LightSource ls)
        {
            if (!_lightSources.Remove(ls))
                Debug.LogError($"Light Source: {ls.name} not present!");
        }

        public void UnselectAll()
        {
            foreach (var ls in _lightSources)
                ls.Unselect();
        }
        
    }
}
