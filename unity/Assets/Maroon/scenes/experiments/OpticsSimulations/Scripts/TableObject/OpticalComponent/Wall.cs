using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Wall : OpticalComponent
    {
        [Header("Aperture Settings")] 
        [SerializeField] private Vector3 r0;
        [SerializeField] private Vector3 n;

        public Vector3 R0
        {
            get => r0;
            set => r0 = value;
        }

        public Vector3 N
        {
            get => n;
            set => n = value;
        }
    }
}
