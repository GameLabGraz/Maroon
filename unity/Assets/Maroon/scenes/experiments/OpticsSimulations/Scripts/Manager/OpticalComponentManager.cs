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
        [SerializeField] private Aperture aperture;
        [SerializeField] private Eye eye;
        [SerializeField] private Lens lens;
        [SerializeField] private TableObject.OpticalComponent.Mirror mirror;
        [SerializeField] private Wall wall;

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
            AddWalls();
            AddOpticalComponent(aperture, new Vector3(1.6f, 0, 0.5f));
            AddOpticalComponent(mirror, new Vector3(2f, 0, 1f));
        }

        private void AddWalls()
        {
            // _walls[0] = Instantiate(wall, new Vector3(2.0f, 0.0f, 1.0f)  + Constants.TableBaseOffset, Quaternion.Euler(0, 0, 0), tableLowLeftCorner.transform); // Bottom wall
            // _walls[1] = Instantiate(wall, new Vector3(2.0f, 2.0f, 1.0f)  + Constants.TableBaseOffset, Quaternion.Euler(0, 0, 180), tableLowLeftCorner.transform); // Top wall
            // _walls[2] = Instantiate(wall, new Vector3(-0.5f, 1.0f, 1.0f) + Constants.TableBaseOffset, Quaternion.Euler(0, 0, -90), tableLowLeftCorner.transform); // Left wall
            // _walls[3] = Instantiate(wall, new Vector3(4.5f, 1.0f, 1.0f)  + Constants.TableBaseOffset, Quaternion.Euler(0, 0, 90), tableLowLeftCorner.transform); // Right wall
            // _walls[4] = Instantiate(wall, new Vector3(2.0f, 1.0f, 2.5f)  + Constants.TableBaseOffset, Quaternion.Euler(-90, 0, 0), tableLowLeftCorner.transform); // Back wall
            // _walls[5] = Instantiate(wall, new Vector3(2.0f, 1.0f, -0.5f) + Constants.TableBaseOffset, Quaternion.Euler(90, 0, 0), tableLowLeftCorner.transform); // Front wall
            //
            // foreach (var w in _walls)
            //     _opticalComponents.Add(w);

            // Bottom wall
            var wBot = Instantiate(wall, tableLowLeftCorner.transform);
            wBot.SetProperties(new Vector3(2.0f, 0.0f, 1.0f), Vector3.up);
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

        public void AddOpticalComponent(OpticalComponent oc, Vector3 pos)
        {
            var ocClone = Instantiate(oc, tableLowLeftCorner.transform);
            ocClone.transform.localPosition = pos;
            ocClone.UpdateProperties();
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
        
        public OpticalComponent GetFirstHitComponent(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float dmin = Mathf.Infinity;
            OpticalComponent firstOc = _opticalComponents[0];
            
            foreach (var oc in _opticalComponents)
            {
                // float d = oc.IsHit(rayOrigin, rayDirection);
                (Vector3 hitPoint, Vector3 outRayDirection) = oc.CalculateHitPointAndOutRayDirection(rayOrigin, rayDirection);

                if (Util.Math.IsValidPoint(hitPoint))
                {
                    float d = Vector3.Distance(rayOrigin, hitPoint);
                    if (d > Constants.Epsilon && d < dmin)
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
