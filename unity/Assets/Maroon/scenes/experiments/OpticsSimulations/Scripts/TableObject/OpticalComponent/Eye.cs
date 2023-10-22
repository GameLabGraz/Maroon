using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Eye : OpticalComponent
    {
        [Header("Eye Settings")] 
        [SerializeField] private float f;
        [SerializeField] private float r;

        public float F
        {
            get => f;
            set => f = value;
        }

        public float R
        {
            get => r;
            set => r = value;
        }
        
        public override void UpdateProperties()
        {
            throw new NotImplementedException();
        }
    }
}
