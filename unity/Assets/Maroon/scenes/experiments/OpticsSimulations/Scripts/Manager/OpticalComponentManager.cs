using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class OpticalComponentManager : MonoBehaviour
    {
        public static OpticalComponentManager Instance;

        private List<OpticalComponent> _opticalComponents;
        [SerializeField] private GameObject tableLowLeftCorner;
        
        [Header("Prefabs: Optical Components")] 
        [SerializeField] private Aperture aperture;
        [SerializeField] private Eye eye;
        [SerializeField] private Lens lens;
        [SerializeField] private TableObject.OpticalComponent.Mirror mirror;
        [SerializeField] private Wall wall;

        public List<OpticalComponent> OpticalComponents => _opticalComponents;
        private Vector3 _basePosition = new Vector3(2, 0, 1);

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
            AddWalls();
            // AddOpticalComponent(mirror, new Vector3(1.5f, 0, 0.30f));
            // AddOpticalComponent(mirror, new Vector3(1.8f, 0, 0.30f));
            //
            // AddOpticalComponent(aperture, new Vector3(2.1f, 0, 0.30f));
            // AddOpticalComponent(aperture, new Vector3(2.4f, 0, 0.30f));
            //
            // AddOpticalComponent(eye, new Vector3(2.7f, 0, 0.30f));
            // AddOpticalComponent(eye, new Vector3(3.0f, 0, 0.30f));
            //
            // AddOpticalComponent(lens, new Vector3(1f, 0, 0.30f));
        }

        private void AddWalls()
        {
            // Bottom wall
            var wBot = Instantiate(wall, tableLowLeftCorner.transform);
            wBot.SetProperties(new Vector3(2.0f, -Constants.TableObjectHeight, 1.0f), Vector3.up);
            _opticalComponents.Add(wBot);
            
            // Top wall
            var wTop = Instantiate(wall, tableLowLeftCorner.transform);
            wTop.SetProperties(new Vector3(2.0f, 2.0f, 1.0f), Vector3.down);
            _opticalComponents.Add(wTop);
            
            // Left wall
            var wL = Instantiate(wall, tableLowLeftCorner.transform);
            wL.SetProperties(new Vector3(-0.5f, 1.0f, 1.0f), Vector3.right);
            _opticalComponents.Add(wL);
            
            // Right wall
            var wR = Instantiate(wall, tableLowLeftCorner.transform);
            wR.SetProperties(new Vector3(4.5f, 1.0f, 1.0f), Vector3.left);
            _opticalComponents.Add(wR);
            
            // Back wall
            var wBack = Instantiate(wall, tableLowLeftCorner.transform);
            wBack.SetProperties(new Vector3(2.0f, 1.0f, 2.5f), Vector3.forward);
            _opticalComponents.Add(wBack);
            
            // Front wall
            var wFront = Instantiate(wall, tableLowLeftCorner.transform);
            wFront.SetProperties(new Vector3(2.0f, 1.0f, -0.5f), Vector3.back);
            _opticalComponents.Add(wFront);
        }

        public void AddOpticalComponent(OpticalComponent oc, Vector3 pos, Vector3? rot = null)
        {
            Vector3 rotation = rot ?? new Vector3(0, 0, 0);
            
            var ocClone = Instantiate(oc, tableLowLeftCorner.transform);
            ocClone.transform.localPosition = pos;
            ocClone.transform.eulerAngles = rotation;
            ocClone.UpdateProperties();
            _opticalComponents.Add(ocClone);
        }
        
        public void AddOC(int nr)
        {
            switch (nr)
            {
                case 0:
                    return;
                case 1:
                    AddOpticalComponent(lens, _basePosition);
                    break;
                case 2:
                    AddOpticalComponent(mirror, _basePosition);
                    break;
                case 3:
                    AddOpticalComponent(eye, _basePosition);
                    break;
                case 4:
                    AddOpticalComponent(aperture, _basePosition);
                    break;
            }
        }

        public void RemoveSelectedOC()
        {
            OpticalComponent selectedOc = UIManager.Instance.SelectedOc;

            if (selectedOc != null)
            {
                _opticalComponents.Remove(selectedOc);
                selectedOc.RemoveFromTable();
                
                LightComponentManager.Instance.RecalculateAllLightRoutes();
                UIManager.Instance.SelectedOc = null;
                UIManager.Instance.DeactivateAllOpticalControlPanels();
            }
        }

        public void UnselectAll()
        {
            foreach (var oc in _opticalComponents)
            {
                if (oc.OpticalType != OpticalType.Wall)
                    oc.GetComponent<SelectionHandler>().Unselect();
            }
        }
        
        public OpticalComponent GetFirstHitComponent(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float dmin = Mathf.Infinity;
            OpticalComponent firstOc = _opticalComponents[0];

            // List<(OpticalComponent oc, float dist)> debugComponentDistances = new List<(OpticalComponent oc, float dist)>();
            
            foreach (var oc in _opticalComponents)
            {
                float d = oc.GetRelevantDistance(rayOrigin, rayDirection);
                // debugComponentDistances.Add((oc, d));
                
                if (Util.Math.IsValidDistance(d))
                {
                    if (d < dmin)
                    {
                        dmin = d;
                        firstOc = oc;
                    }
                }
            }
            
            return firstOc;
        }
        
    }
}
