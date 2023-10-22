using System;
using System.Collections;
using System.Collections.Generic;
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
            n = Vector3.right;
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
            // return Vector3.Normalize(r + this.r - (r + (R * n)));
            return Vector3.Normalize(r - (this.r + (R * n)));
            // let vec = vsub(r,vadd(this.r,dot(this.R,this.n)));
            // return unit(vec);
        }
        
         // center of Mirror
        public Vector3 Center()
        {
            return r + R * n;
            // return r;
        }
        
        
        public override void UpdateProperties()
        {
            r = transform.localPosition;
        }

        public override (Vector3 hitPoint, Vector3 surfaceNormal) CalculateHitPointAndNormal(Vector3 incRayPos, Vector3 incRayDir)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(incRayPos, incRayDir, R, Center());

            float d;
            if (d1 > 0 && d2 > 0)
                d = d1 < d2 ? d1 : d2;
            else if (d1 <= 0)
                d = d2;
            else
                d = d1;

            Vector3 firstHit = incRayPos + incRayDir * d;
            return (firstHit, NormR(firstHit));
        }
        //
        // dc() { //distance along cylinder from central plane to intersection with surface R
        //     let absR = Math.abs(this.R);
        //     return absR - Math.sqrt(absR*absR - this.Rc*this.Rc);
        // }
        //
        // center() { // center of curvature
        //     return vadd(this.r,dot(this.R,this.n));
        // }
        //
        // adjust_Rc() {
        //     return Math.min(this.Rc,Math.abs(this.R)); 
        // } 
        
    }
}
