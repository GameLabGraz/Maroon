using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    [System.Serializable]
    public class ApertureParameters : OpticalComponentParameters
    {
        public float Rin = 0.03f;
        public float Rout = 0.10f;
    }
}