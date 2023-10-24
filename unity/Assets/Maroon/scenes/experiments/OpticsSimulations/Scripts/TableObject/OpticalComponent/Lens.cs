using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Lens : OpticalComponent
    {
        [Header("Lens Properties")] 
        public float r1;
        public float r2;
        public float rc;
        public float a;
        public float b;

        public override void UpdateProperties()
        {
            throw new NotImplementedException();
        }

        public override (Vector3 hitPoint, Vector3 outRayReflection, Vector3 outRayRefraction) CalculateHitpointReflectionRefraction(Vector3 rayOrigin, Vector3 rayDirection)
        {
            throw new NotImplementedException();
        }
        

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            throw new NotImplementedException();
        }

    }
}
