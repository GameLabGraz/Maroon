using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Wall : OpticalComponent
    {
        [Header("Wall Properties")] 
        public Vector3 r0;
        public Vector3 n;

        public void SetProperties(Vector3 r0, Vector3 n)
        {
            this.r0 = r0;
            this.n = n;
        }

        public override (Vector3 hitPoint, Vector3 surfaceNormal) CalculateHitPointAndNormal(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float d = Util.Math.IntersectLinePlane(rayOrigin, rayDirection, r0, n);
            return (rayOrigin + rayDirection * d, n);
        }
        
        public override float IsHit(Vector3 rayOrigin, Vector3 rayDirection)
        {
            return Util.Math.IntersectLinePlane(rayOrigin, rayDirection, r0, n);
        }
        
        public override void UpdateProperties()
        {
        }
    }
}
