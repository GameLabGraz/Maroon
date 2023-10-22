using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Wall : OpticalComponent
    {
        [Header("Wall Properties")] 
        public Vector3 r0;
        public Vector3 n;

        private void Start()
        {
            r0 = transform.localPosition;
            n = transform.up;
        }
        
        
        public override (Vector3 hitPoint, Vector3 surfaceNormal) CalculateHitPointAndNormal(Vector3 incRayPos, Vector3 incRayDir)
        {
            float d = Util.Math.IntersectLinePlane(incRayPos, incRayDir, r0, n);
            return (incRayPos + incRayDir * d, n);
        }
    }
}
