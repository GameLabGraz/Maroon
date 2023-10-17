using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Aperture : OpticalComponent
    {
        [Header("Aperture Settings")] 
        [SerializeField] private float rin;
        [SerializeField] private float rout;

        public float Rin
        {
            get => rin;
            set => rin = value;
        }

        public float Rout
        {
            get => rout;
            set => rout = value;
        }
    }
}
