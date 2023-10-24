using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Eye : OpticalComponent
    {
        [Header("Eye Properties")] 
        public Vector3 r0;
        public Vector3 n;
        public float f;
        public float R = 2.4f/100;
        
        // TODO make sure it is correctly implemented
        // normal to surface R at p
        public Vector3 NormR(Vector3 p)
        { 
            return 1/this.R * (p - this.r0);
        }
        
        private void Start()
        {
            r0 = transform.localPosition;
            n = transform.right;
            f = 0.25f;
        }
        
        public override void UpdateProperties()
        {
            r0 = transform.localPosition;
        }
        
        private void FixedUpdate()
        {
            r0 = transform.localPosition;
            n = transform.right;
        }
        
        public override (Vector3 hitPoint, Vector3 outRayReflection, Vector3 outRayRefraction) CalculateHitpointReflectionRefraction(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float dmin = GetRelevantDistance(rayOrigin, rayDirection);
            
            // todo implement full eye behaviour
            
            return (rayOrigin + rayDirection * dmin, Vector3.zero, Vector3.zero);
        }

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, R, r0);
            float dmin = Mathf.Infinity;
            
            // skip if (d1 or d2) is negative, very small or NaN
            if (d1 > Constants.Epsilon && !float.IsNaN(d1) && d1 < dmin)
                dmin = d1;
            if (d2 > Constants.Epsilon && !float.IsNaN(d2) && d2 < dmin)
                dmin = d2;
            return dmin;
        }
        
    }
}
