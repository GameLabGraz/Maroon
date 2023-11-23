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
        
        public int numberOfRays = 40;
        
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
            // foreach (var lr in _lightRoutes)
            //     lr.ResetLightRoute();
            
            ClearLightRoutes();
            for (int i = 0; i < numberOfRays; i++)
                _lightRoutes.Add(new LightRoute(Wavelength));
            
            RecalculateLightRoute();
        }

        private void ClearLightRoutes()
        {
            _lightRoutes.ForEach(lr => lr.ResetLightRoute());
            _lightRoutes.Clear();

            for (int i = 0; i < numberOfRays; i++)
                _lightRoutes.Add(new LightRoute(Wavelength));
        }
        
        
        // public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        // {
        //     throw new NotImplementedException("CheckHitComponent Method not implemented for PointSource!");
        //     // return false;
        // }
        
        public override void RecalculateLightRoute()
        {
            if (_lightRoutes == null)
                return;
            ClearLightRoutes();

            float angle = 0;
            float angleDelta = 360f / numberOfRays;

            for (int i = 0; i < numberOfRays; i++)
            {
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.right;
                var initialRay = new RaySegment(Origin, Intensity, Wavelength, transform.rotation * dir);
                _lightRoutes[i].AddRaySegment(initialRay);
                _lightRoutes[i].CalculateNextRay(initialRay);
                
                angle += angleDelta;
            }
            
            
        }
        
        public override void RemoveFromTable()
        {
            foreach (var lr in _lightRoutes)
                lr.ResetLightRoute();
            Destroy(gameObject);
        }
        
    }
}