using System;
using Maroon.Physics.Optics.Light;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    public class OpticalComponent : TableObject
    {
        [SerializeField] private OpticalCategory opticalCategory;
        
        public OpticalCategory OpticalCategory => opticalCategory;

        public virtual (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            throw new Exception("Should not call base CalculateHitpointReflectionRefraction Method!");
        }
        
        public virtual float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            throw new Exception("Should not call base GetRelevantDistance Method!");
        }
        
        public virtual void UpdateProperties()
        {
            throw new Exception("Should not call base UpdateProperties Method!");
        }
        
        public override void RemoveFromTable()
        {
            Destroy(gameObject);
        }

    }
    
    public enum OpticalCategory
    {
        Undefined,
        Lens,
        Mirror,
        Eye,
        Aperture,
        Wall
    }
}
