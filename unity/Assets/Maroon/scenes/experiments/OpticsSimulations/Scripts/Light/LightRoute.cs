using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Light
{
    public class LightRoute
    {
        [Header("Light Settings")]
        [SerializeField] private float intensity;
        [SerializeField] private float wavelength;
        
        private List<RaySegment> _raySegments = new List<RaySegment>();

        public List<RaySegment> RaySegments
        {
            get => _raySegments;
            set => _raySegments = value;
        }
        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }
        public float Wavelength
        {
            get => wavelength;
            set => wavelength = value;
        }

        public LightRoute(float intensity, float wavelength)
        {
            this.intensity = intensity;
            this.wavelength = wavelength;
        }

        public void ResetLightRoute(float wavelength)
        {
            intensity = 1.0f;
            this.wavelength = wavelength;

            for (int i = 0; i < _raySegments.Count; i++)
            {
                var tmpSegment = _raySegments[i];
                _raySegments.Remove(tmpSegment);
                tmpSegment.DestroyRaySegment();
            }
        }
        
        public OpticalComponent GetHitComponent(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Vector3 globalRayOrigin =  rayOrigin + Constants.TableBaseOffset + new Vector3(0, Constants.TableObjectHeight, 0);
            if (UnityEngine.Physics.Raycast(globalRayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity, ~(1 << 4)))
                return hit.transform.gameObject.GetComponent<OpticalComponent>();
            
            Debug.LogError("Did not hit any object - can not occur!");
            return null;
        }

        public void CalculateNextRay(Vector3 rayOrigin, Vector3 rayDirection)
        {
            // Get the first hit component via raycast, so we do not need to go through every Component on the table
            OpticalComponent hitComponent = GetHitComponent(rayOrigin, rayDirection);

            switch (hitComponent.OpticalType)
            {
                // End of ray - no further reflection/refraction
                case OpticalType.Wall:
                    Wall wall = (Wall) hitComponent;

                    (Vector3 hitPointW, _) = wall.CalculateHitPointAndNormal(rayOrigin, rayDirection);
                    // Debug.Log(wall.name+" Hit " + hitPointW.ToString("f3"));

                    AddRaySegment(rayOrigin, hitPointW);
                    break;
                        
                // Ray has 1 further reflection
                case OpticalType.Mirror:
                    
                    TableObject.OpticalComponent.Mirror mirror = (TableObject.OpticalComponent.Mirror) hitComponent;

                    (Vector3 hitPointM, Vector3 normalM) = mirror.CalculateHitPointAndNormal(rayOrigin, rayDirection);
                    Debug.Log("MIRROR Hit " + hitPointM.ToString("f3"));
                    Debug.Log("MIRROR n   " + normalM.ToString("f3"));

                    AddRaySegment(rayOrigin, hitPointM);
                    CalculateNextRay(hitPointM, normalM);
                    break;
                        
                case OpticalType.Aperture:
                    throw new NotImplementedException("Aperture calculations not implemented yet!");
                case OpticalType.Eye:
                    throw new NotImplementedException("Eye calculations not implemented yet!");
                case OpticalType.Lens:
                    throw new NotImplementedException("Lens calculations not implemented yet!");
            }
        }
        
        public RaySegment AddRaySegment(Vector3 origin, Vector3 endpoint)
        {
            // TODO intensity calculation
            var rs = new RaySegment(origin, endpoint, intensity * 0.9f, wavelength);
            _raySegments.Add(rs);
            return rs;
        }

        public RaySegment GetFirstClearRest()
        {
            if (_raySegments.Count == 0)
            {
                Debug.LogError("LightRoute can not consist of zero rays!");
                return null;
            }

            for (int i = 1; i < _raySegments.Count; i++)
            {
                var tmpSegment = _raySegments[i];
                tmpSegment.DestroyRaySegment();
                _raySegments.Remove(tmpSegment);

            }
            // _raySegments.RemoveRange(1, _raySegments.Count - 1);
            return _raySegments[0];
        }
        

    }
}
