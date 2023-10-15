//
//Author: Alexander Kassil
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightSource
{
    public class SingleRay : LightSource
    {
        [SerializeField] private List<RaySegment> raySegments;

        public List<RaySegment> RaySegments
        {
            get => raySegments;
            set => raySegments = value;
        }

        void Start()
        {
            
        }

    }
}
