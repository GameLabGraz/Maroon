using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
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
        
        public override void UpdateProperties()
        {
            Debug.Log("Updated properties should never have to be called for Wall componentes!");
        }

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

        // Walls are not removed
        public override void RemoveFromTable() {}
        
    }
}
