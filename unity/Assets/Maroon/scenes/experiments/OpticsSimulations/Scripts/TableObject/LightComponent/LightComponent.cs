using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class LightComponent : TableObject
    {
        [SerializeField] private LightType lightType;
        
        [Header("Light Properties")] 
        [SerializeField] private float intensity;
        [SerializeField] private float wavelength;

        public LightType LightType => lightType;
        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }
        public float Wavelength
        {
            get => wavelength;
            set => wavelength = value;
        }

        // private void Update()
        // {
        //     if (transform.hasChanged)
        //     {
        //         
        //         LightComponentManager.Instance.CalculateRays(this);
        //         transform.hasChanged = false;
        //     }
        // }

    }
    
    public enum LightType
    {
        LaserPointer = 0,
        PointSource = 1,
        ParallelSource = 2
    }
}
