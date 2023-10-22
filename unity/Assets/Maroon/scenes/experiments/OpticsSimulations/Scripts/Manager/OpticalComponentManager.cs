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

        private List<OpticalComponent> _opticalComponents;
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
            _opticalComponents = new List<OpticalComponent>();
        }

        private void Start()
        {
            
            // Spawn 2 mirrors for testing
            AddOpticalComponent(mirror, new Vector3(2f, 0, 0.9f));
            // AddOpticalComponent(mirror, new Vector3(2.2f, 0, 0.20f));
        }

        public void AddOpticalComponent(OpticalComponent oc, Vector3 pos)
        {
            var ocClone = Instantiate(oc, tableLowLeftCorner.transform);
            ocClone.transform.localPosition = pos;
            _opticalComponents.Add(ocClone);
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
        
        public OpticalComponent GetHitComponent(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Vector3 globalRayOrigin =  rayOrigin + Constants.TableBaseOffset + new Vector3(0, Constants.TableObjectHeight, 0);
            if (UnityEngine.Physics.Raycast(globalRayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity, ~(1 << 4)))
                return hit.transform.gameObject.GetComponent<OpticalComponent>();
            
            Debug.LogError("Did not hit any object - can not occur!");
            return null;
        }
        
        
        
    }
}
