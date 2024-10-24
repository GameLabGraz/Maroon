using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Physics.Optics.TableObject.LightComponent
{
    [System.Serializable]
    public class LightComponentParameters : TableObjectParameters
    {
        public List<float> waveLengths;
    }
}