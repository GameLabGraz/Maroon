using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class PointSource : LightComponent
    {
        [Header("Parallel Source Settings")] 
        private List<LightRoute> _lightRoutes;
        
        public int numberOfRays = 20;
        
        private void Start()
        {
            _lightRoutes = new List<LightRoute>();
            Origin = transform.localPosition;

            for (int i = 0; i < numberOfRays; i++)
                _lightRoutes.Add(new LightRoute(Wavelength));

            RecalculateLightRoute();
        }

        public void ChangeNumberOfRays(int nrRays)
        {
            numberOfRays = nrRays;
            _lightRoutes.Clear();
            
            for (int i = 0; i < numberOfRays; i++)
                _lightRoutes.Add(new LightRoute(Wavelength));
        }
        
        
        public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        {
            throw new NotImplementedException("CheckHitComponent Method not implemented for PointSource!");
            // return false;
        }
        
        public override void RecalculateLightRoute()
        {
            throw new NotImplementedException("RecalculateLightRoute Method not implemented for PointSource!");
        }
        
        public override void RemoveFromTable()
        {
            foreach (var lr in _lightRoutes)
                lr.ResetLightRoute();
            Destroy(gameObject);
        }
        
    }
}