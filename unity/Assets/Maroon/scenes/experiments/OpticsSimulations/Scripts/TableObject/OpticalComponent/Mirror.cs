using System;
using System.Collections;
using System.Collections.Generic;
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
            r = transform.localPosition;
            n = Vector3.left;
            R = 0.1f;
            Rc = 0.09f;

        }

        private void FixedUpdate()
        {
            transform.localScale = Vector3.one * R;
        }

        // normal to surface at r
        public Vector3 NormR(Vector3 r)
        {
            return Vector3.Normalize(r - (this.r + (this.R * this.n)));
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
            return r + R * n;
            // return r;
        }
        public float adjust_Rc()
        {
            return Mathf.Min(this.Rc,Mathf.Abs(this.R)); 
        } 
        
        
        
        public override void UpdateProperties()
        {
            r = transform.localPosition;
        }

        public override (Vector3 hitPoint, Vector3 surfaceNormal) CalculateHitPointAndNormal(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, R, Center());

            float d;
            if (d1 > 0 && d2 > 0)
                d = d1 < d2 ? d1 : d2;
            else if (d1 <= 0)
                d = d2;
            else
                d = d1;

            Vector3 firstHit = rayOrigin + rayDirection * d;
            return (firstHit, NormR(firstHit));
        }
        
        public override float IsHit(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, R, Center());

            float d;
            if (d1 > 0 && d2 > 0)
                d = d1 < d2 ? d1 : d2;
            else if (d1 <= 0)
                d = d2;
            else
                d = d1;
            return d;
        }
        
    }
}
