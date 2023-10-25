using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Light
{
    public class LightRoute
    {
        [Header("Light Settings")]
        [SerializeField] private float wavelength;
        
        private List<RaySegment> _raySegments = new List<RaySegment>();

        public List<RaySegment> RaySegments
        {
            get => _raySegments;
            set => _raySegments = value;
        }
        public float Wavelength
        {
            get => wavelength;
            set => wavelength = value;
        }

        public LightRoute(float intensity, float wavelength)
        {
            this.wavelength = wavelength;
        }

        public void ResetLightRoute()
        {
            foreach (var rs in _raySegments)
            {
                rs.DestroyRaySegment();
            }

            _raySegments.Clear();
        }
        
        public void CalculateNextRay(RaySegment inRay)
        {
            // Stop when maximal number of rays (per light route) is reached
            if (_raySegments.Count >= Constants.MaxNumberOfRays)
                return;
            
            // Get the first hit component via raycast, so we do not need to go through every Component on the table
            OpticalComponent hitComponent = OpticalComponentManager.Instance.GetFirstHitComponent(inRay.r0Local, inRay.n);
            
            switch (hitComponent.OpticalType)
            {
                // End of ray - no further reflection/refraction
                case OpticalType.Wall:
                    Wall wall = (Wall) hitComponent;
                    (float distanceToWall, _, _) = wall.CalculateDistanceReflectionRefraction(inRay);
                    
                    inRay.UpdateLength(distanceToWall);
                    break;
                
                case OpticalType.Aperture:
                    Aperture aperture = (Aperture)hitComponent;
                    (float distanceToAperture, _, _) = aperture.CalculateDistanceReflectionRefraction(inRay);
                    
                    inRay.UpdateLength(distanceToAperture);
                    break;
                        
                // Ray has 1 further reflection
                case OpticalType.Mirror:
                    TableObject.OpticalComponent.Mirror mirror = (TableObject.OpticalComponent.Mirror) hitComponent;
                    (float distanceToMirror, RaySegment reflectionM, _) = mirror.CalculateDistanceReflectionRefraction(inRay);
                    
                    inRay.UpdateLength(distanceToMirror);
                    AddRaySegment(reflectionM);
                    CalculateNextRay(reflectionM);
                    break;

                // Ray has potential reflection and refraction
                case OpticalType.Eye:
                    Eye eye = (Eye)hitComponent;
                    (float distanceToEye, RaySegment reflectionEye, RaySegment refractionEye) = eye.CalculateDistanceReflectionRefraction(inRay);

                    inRay.UpdateLength(distanceToEye);
                    if (reflectionEye != null)
                    {
                        AddRaySegment(reflectionEye);
                        CalculateNextRay(reflectionEye);
                    }

                    if (refractionEye != null)
                    {
                        AddRaySegment(refractionEye);
                        CalculateNextRay(refractionEye);
                    }
                    break;
                
                // Ray has reflection and refraction
                case OpticalType.Lens:
                    Lens lens = (Lens) hitComponent;
                    (float distanceToLens, RaySegment reflectionLens, RaySegment refractionLens) = lens.CalculateDistanceReflectionRefraction(inRay);
                    Debug.Log("lens dist: "+ distanceToLens.ToString("F3"));
                    inRay.UpdateLength(distanceToLens);
                    if (reflectionLens != null)
                    {
                        AddRaySegment(reflectionLens);
                        CalculateNextRay(reflectionLens);
                    }

                    if (refractionLens != null)
                    {
                        AddRaySegment(refractionLens);
                        CalculateNextRay(refractionLens);
                    }
                    break;
            }
        }
        
        public RaySegment AddRaySegment(Vector3 origin, Vector3 endpoint, float intensity)
        {
            // TODO intensity calculation
            var rs = new RaySegment(origin, endpoint, intensity, wavelength);
            _raySegments.Add(rs);
            return rs;
        }

        public void AddRaySegment(RaySegment rs)
        {
            _raySegments.Add(rs);
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
            return _raySegments[0];
        }
        

    }
}
