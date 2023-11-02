using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Aperture : OpticalComponent
    {
        [Header("Aperture Properties")] 
        public Vector3 r;
        public Vector3 n;
        public float Rin;
        public float Rout;

        private void Start()
        {
            Rin = 0.1f;
            Rout = 0.3f;
            UpdateProperties();
            LightComponentManager.Instance.CheckOpticalComponentHit(this);
        }

        public override void UpdateProperties()
        {
            r = transform.localPosition;
            n = transform.right;
        }
        
        // public override (Vector3 hitPoint, Vector3 outRayReflection, Vector3 outRayRefraction) CalculateHitpointReflectionRefraction(Vector3 inRayOrigin, Vector3 inRayDirection)
        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);
            return (d, null, null);
        }
        
        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float d = Util.Math.IntersectLinePlane(rayOrigin, rayDirection, r, n);
            float dmin = Mathf.Infinity;
            
            // skip if d is negative, very small or NaN
            if (d > Constants.Epsilon && !float.IsNaN(d)) 
            {
                var _r = rayOrigin + d * rayDirection;
                var r1 = _r - this.r;
                    
                if (this.Rin < r1.magnitude && r1.magnitude < this.Rout && d < dmin)
                    dmin = d;
            }

            return dmin;
        }
    }
}
