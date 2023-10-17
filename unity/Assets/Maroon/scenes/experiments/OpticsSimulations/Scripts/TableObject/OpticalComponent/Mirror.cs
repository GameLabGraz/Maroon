using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Mirror : OpticalComponent
    {
        [Header("Mirror Settings")] 
        [SerializeField] private float r;
        [SerializeField] private float rc;

        public float R
        {
            get => r;
            set => r = value;
        }

        public float Rc
        {
            get => rc;
            set => rc = value;
        }
    }
}
