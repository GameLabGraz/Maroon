using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;
using LightType = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LightType;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class LightComponentManager : MonoBehaviour
    {
        public static LightComponentManager Instance;

        private List<LightComponent> _lightComponents;
        [SerializeField] private GameObject tableLowLeftCorner;
        
        [Header("Prefabs: Light Sources")] 
        [SerializeField] private LightComponent laserPointer;

        public List<LightComponent> LightComponents => _lightComponents;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed LightSourceManager");
                Destroy(gameObject);
            }
            _lightComponents = new List<LightComponent>();
        }

        private void Start()
        {
            // Spawn 2 lasers for testing
            // AddLightComponent(laserPointer, new Vector3(1.74f, 0, 0.5f));
            AddLightComponent(laserPointer, new Vector3(1.70f,0,1.09f));
        }

        public void CheckOpticalComponentHit(OpticalComponent opticalComponent)
        {
            foreach (var lightComponent in _lightComponents)
                lightComponent.RecalculateLightRoute();
        }

        public void AddLightComponent(LightComponent lc, Vector3 pos)
        {
            var lsClone = Instantiate(lc, tableLowLeftCorner.transform);
            lsClone.transform.localPosition = pos;
            _lightComponents.Add(lsClone);
        }

        public void RemoveLightComponent(LightComponent lc)
        {
            if (!_lightComponents.Remove(lc))
                Debug.LogError($"Light Source: {lc.name} not present!");
        }

        public void UnselectAll()
        {
            foreach (var ls in _lightComponents)
                ls.Unselect();
        }
        
    }
}
