using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    [System.Serializable]
    public class LensParameters : OpticalComponentParameters
    {
        public float R1 = 0.15f;
        public float R2 = -0.15f;
        public float d1 = 0.02f;
        public float d2 = 0.02f;
        public float Rc = 0.065f;
        public float A = 1.728f;
        public float B = 13420f;
    }
}