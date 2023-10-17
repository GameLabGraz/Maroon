using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class OpticalComponentManager : MonoBehaviour
    {
        public static OpticalComponentManager Instance;

        [SerializeField] private List<OpticalComponent> _opticalComponents; // TODO dont serialize after testing is finished
        [SerializeField] private GameObject tableLowLeftCorner;
        
        [Header("Prefabs: Optical Components")] 
        [SerializeField] private OpticalComponent aperture;
        [SerializeField] private OpticalComponent eye;
        [SerializeField] private OpticalComponent lens;
        [SerializeField] private OpticalComponent mirror;

        public List<OpticalComponent> OpticalComponents => _opticalComponents;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed OpticalComponentManager");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
        }

        public void AddOpticalComponent(OpticalComponent oc)
        {
            _opticalComponents.Add(oc);
        }

        public void RemoveOpticalComponent(OpticalComponent oc)
        {
            if (!_opticalComponents.Remove(oc))
                Debug.LogError($"Optical Component: {oc.name} not present!");
        }

        public void UnselectAll()
        {
            foreach (var oc in _opticalComponents)
                oc.Unselect();
        }
        
    }
}
