using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class OpticalComponent : TableObject
    {
        [SerializeField] private OpticalType opticalType;
        
        public OpticalType OpticalType => opticalType;

        public virtual (Vector3 hitPoint, Vector3 surfaceNormal) CalculateHitPointAndNormal(Vector3 rayOrigin, Vector3 rayDirection)
        {
            throw new NotImplementedException("Should not call base CalculateHitPoint Method!");
        }
        
        private void Update()
        {
            if (transform.hasChanged)
            {
                
                transform.hasChanged = false;
            }
        }

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
