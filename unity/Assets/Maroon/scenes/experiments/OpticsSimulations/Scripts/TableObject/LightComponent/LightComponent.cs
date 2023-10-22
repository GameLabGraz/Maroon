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

        private Vector3 _origin;

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
        public Vector3 Origin
        {
            get => _origin;
            set => _origin = value;
        }

        
        private void Update()
        {
            if (transform.hasChanged)
            {
                _origin = transform.localPosition;
                RecalculateLightRoute();
                transform.hasChanged = false;
            }
        }
        
        public virtual bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        {
            throw new Exception("Should not call base CheckHitComponent Method!");
        }
        
        public virtual void RecalculateLightRoute()
        {
            throw new Exception("Should not call base RecalculateLightRoute Method!");
        }

    }
    
    public enum LightType
    {
        LaserPointer = 0,
        PointSource = 1,
        ParallelSource = 2
    }
}
