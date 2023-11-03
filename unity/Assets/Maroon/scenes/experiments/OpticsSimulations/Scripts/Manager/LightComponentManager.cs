using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;
using LaserPointer = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LaserPointer;
using LightType = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LightType;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class LightComponentManager : MonoBehaviour
    {
        public static LightComponentManager Instance;

        private List<LightComponent> _lightComponents;
        [SerializeField] private GameObject tableLowLeftCorner;
        
        [Header("Prefabs: Light Sources")] 
        [SerializeField] private LaserPointer laserPointer;
        [SerializeField] private ParallelSource parallelSource;


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
            // SpawnLaserPointerTestSetup();

            // AddLightComponent(laserPointer, new Vector3(1.74f, 0, 0.5f));
            // laserPointer.Wavelength = 720;
            AddLightComponent(laserPointer, new Vector3(1.70f,0,1.09f));
            //
            // parallelSource.numberOfRays = 10;
            // AddLightComponent(parallelSource, new Vector3(1.70f,0,1.0f));
            //
            // parallelSource.numberOfRays = 30;
            AddLightComponent(parallelSource, new Vector3(1.70f,0,0.6f));
        }

        private void SpawnLaserPointerTestSetup()
        {
            int wl = 382;
            for (float i = 0.8f; i <= 1.8f; i+=0.06f)
            {
                laserPointer.Wavelength = wl;
                AddLightComponent(laserPointer, new Vector3(1, 0, i));
                // wl += 20;
                wl += 15;
            }
            
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
                ls.GetComponent<SelectionHandler>().Unselect();
        }

    }
}
