using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
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
            // LightComponentManager.Instance.CheckOpticalComponentHit(this);
        }
        public override void UpdateProperties()
        {
            Debug.Log("Updated properties should never have to be called for Wall componentes!");
        }

        // public override (Vector3 hitPoint, Vector3 outRayReflection, Vector3 outRayRefraction) CalculateHitpointReflectionRefraction(Vector3 rayOrigin, Vector3 rayDirection)
        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);
            
            return (d, null, null);
        }

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float d = Util.Math.IntersectLinePlane(rayOrigin, rayDirection, r0, n);
            
            if (d < 0)
                d = Mathf.Infinity; // To know when wall is "behind" laser

            return d;
        }
        
        
    }
}
