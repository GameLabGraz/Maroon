using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Light
{
    public class LightRoute
    {
        [Header("Light Settings")]
        [SerializeField] private Vector3 origin;
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

        public LightRoute(Vector3 origin, float intensity, float wavelength)
        {
            this.origin = origin;
            this.intensity = intensity;
            this.wavelength = wavelength;
            
            var rs = new RaySegment(origin, Vector3.right, intensity, wavelength);
            _raySegments.Add(rs);

            // TODO restructure
            // testing reasons
            var distanceToIntersection =
                Math.Util.IntersectLinePlane(rs.r0, rs.n, new Vector3(4.0f+0.5f, 1.0f, 1f) + Constants.TableBaseOffset, Vector3.left);
            
            Debug.Log("dist: " + distanceToIntersection);
            rs.UpdateLength(distanceToIntersection);
        }

        public void UpdateOrigin(Vector3 newOrigin)
        {
            origin = newOrigin;
            RaySegment first = GetFirstClearRest();
            first.UpdateStartingPoint(origin);
            
            // TODO restructure the intersection thing
            var distanceToIntersection =
                Math.Util.IntersectLinePlane(first.r0, first.n, new Vector3(4.0f+0.5f, 1.0f, 1f) + Constants.TableBaseOffset, Vector3.left);
            
            Debug.Log("dist: " + distanceToIntersection);
            first.UpdateLength(distanceToIntersection);
        }

        private RaySegment GetFirstClearRest()
        {
            _raySegments.RemoveRange(1, _raySegments.Count - 1);
            return _raySegments[0];
        }

    }
}
