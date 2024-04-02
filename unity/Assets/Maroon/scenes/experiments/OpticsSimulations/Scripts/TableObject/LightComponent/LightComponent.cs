using System;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class LightComponent : TableObject
    {
        [SerializeField] private LightType lightType;
        
        [Header("Light Properties")] 
        [SerializeField] private float intensity;
        [SerializeField] private List<float> wavelengths;
        private List<LightPath> _lightRoutes;  // Depending on the number of wavelengths and rays

        private Vector3 _origin;

        public LightType LightType => lightType;
        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }
        public List<float> Wavelengths
        {
            get => wavelengths;
            set => wavelengths = value;
        }
        public List<LightPath> LightRoutes
        {
            get => _lightRoutes;
            set => _lightRoutes = value;
        }
        public Vector3 Origin
        {
            get => _origin;
            set => _origin = value;
        }
        
        protected void ResetLightRoutes(int nrRays)
        {
            if (_lightRoutes == null)
                return;
            
            _lightRoutes.ForEach(lr => lr.ResetLightRoute());
            _lightRoutes.Clear();

            foreach (var wl in Wavelengths)
                for (int i = 0; i < nrRays; i++)
                    _lightRoutes.Add(new LightPath(wl));
        }

        public void ChangeWavelength(List<float> wls)
        {
            wavelengths = wls;
            RecalculateLightRoute();
        }
        
        public void ChangeIntensity(float it)
        {
            intensity = it;
            RecalculateLightRoute();
        }
        
        public virtual void RecalculateLightRoute()
        {
            throw new Exception("Should not call base RecalculateLightRoute Method!");
        }

        public override void RemoveFromTable()
        {
            foreach (var lr in LightRoutes)
                lr.ResetLightRoute();
            Destroy(gameObject);
        }

    }
    
    public enum LightType
    {
        LaserPointer = 0,
        PointSource = 1,
        ParallelSource = 2
    }
}
