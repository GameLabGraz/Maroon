using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts
{
    public class OpticalComponentManager : MonoBehaviour
    {
        public static OpticalComponentManager Instance;

        [SerializeField] private List<OpticalComponent> _opticalComponents; // TODO dont serialize after testing is finished

        public List<OpticalComponent> OpticalComponents => _opticalComponents;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed OpOpticalComponentManager");
                Destroy(gameObject);
            }
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
