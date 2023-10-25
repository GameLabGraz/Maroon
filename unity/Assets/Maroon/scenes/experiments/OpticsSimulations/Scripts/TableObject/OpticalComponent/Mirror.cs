using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Mirror : OpticalComponent
    {
        [Header("Mirror Properties")] 
        public Vector3 r;
        public Vector3 n;
        public float R;
        public float Rc;

        private void Start()
        {
            r = Vector3.zero;
            n = transform.right;
            R = 0.1f;
            Rc = 0.1f;
        }
             
        public override void UpdateProperties()
        {
            r = Center() + R * n;
        }

        private void FixedUpdate()
        {
            transform.localScale = Vector3.one * (R * 2);
            n = transform.right;
        }
        
        // ---- Mirror helper methods ----
        
        // normal to surface at p
        public Vector3 NormR(Vector3 p)
        {
            return (p - Center()).normalized; 
        }
        
        // distance along cylinder from central plane to intersection with surface R
        public float Dc()
        { 
            var absR = Mathf.Abs(this.R);
            return absR - Mathf.Sqrt(absR*absR - this.Rc*this.Rc);
        }
        
        // center of Mirror
        public Vector3 Center()
        {
            return transform.localPosition;
            // return r + R * n;
        }
        public float AdjustRc()
        {
            return Mathf.Min(this.Rc,Mathf.Abs(this.R)); 
        } 
        
        // Calculations from Peter
        // public override (Vector3 hitPoint, Vector3 outRayReflection, Vector3 outRayRefraction) CalculateHitpointReflectionRefraction(Vector3 rayOrigin, Vector3 rayDirection)
        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);

            Vector3 hitPoint = inRay.r0Local + inRay.n * d;
            return (d, CalcReflectedRay(hitPoint, inRay), null);
        }

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, R, Center());
            float dmin = Mathf.Infinity;
            
            // skip if d1 is negative, very small or NaN
            if (d1 > Constants.Epsilon && !float.IsNaN(d1))
            {
                var _r = rayOrigin + d1 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;
                
                // is r within the cylinder?
                if (rperp.magnitude < this.AdjustRc())
                {
                    // close solution
                    if (Mathf.Abs(r1dotnc) < this.Dc())
                        dmin = d1;
                }
            }
            // skip if d2 is negative or vey small
            if (d2 > Constants.Epsilon && !float.IsNaN(d2))
            {
                var _r = rayOrigin + d2 * rayDirection;  
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;
                
                // is r within the cylinder?
                if (rperp.magnitude < this.AdjustRc())
                { 
                    // close solution
                    if (Mathf.Abs(r1dotnc) < this.Dc())
                        if (d2 < dmin) 
                            dmin = d2;
                }
            }

            return dmin;
        }

        private RaySegment CalcReflectedRay(Vector3 hitPoint, RaySegment inRay)
        {
            var normal = this.NormR(hitPoint);
            var reflectedRayDirection = Vector3.Reflect(inRay.n, normal);
            return new RaySegment(hitPoint, inRay.intensity, inRay.wavelength, reflectedRayDirection);
        }
        
         
        
    }
}
