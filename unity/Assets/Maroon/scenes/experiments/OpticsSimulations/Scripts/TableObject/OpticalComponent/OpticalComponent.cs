using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class OpticalComponent : TableObject
    {
        [SerializeField] private OpticalType opticalType;
        
        public OpticalType OpticalType => opticalType;

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
        
        // private void Update()
        // {
        //     if (transform.hasChanged)
        //     {
        //         UpdateProperties();
        //         LightComponentManager.Instance.CheckOpticalComponentHit(this);
        //         transform.hasChanged = false;
        //     }
        // }

    }
    
    public enum OpticalType
    {
        Aperture = 0,
        Eye = 1,
        Lens = 2,
        Mirror = 3,
        Wall = 4,
    }
}
