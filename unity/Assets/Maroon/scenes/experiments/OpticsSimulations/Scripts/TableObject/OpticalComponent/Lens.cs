//
//Author: Alexander Kassil
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Lens : OpticalComponent
    {
        [Header("Lens Settings")] 
        [SerializeField] private float r1;
        [SerializeField] private float r2;
        [SerializeField] private float rc;
        [SerializeField] private float a;
        [SerializeField] private float b;

        public float R1
        {
            get => r1;
            set => r1 = value;
        }

        public float R2
        {
            get => r2;
            set => r2 = value;
        }

        public float Rc
        {
            get => rc;
            set => rc = value;
        }

        public float A
        {
            get => a;
            set => a = value;
        }

        public float B
        {
            get => b;
            set => b = value;
        }
    }
}
