using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.LightComponent
{
    [System.Serializable]
    public class PointSourceParameters : LightComponentParameters
    {
        public int numberOfRays = 16;
        public float rayDistributionAngle = 30;
        public bool enableMeshRenderer = true;
    }
}