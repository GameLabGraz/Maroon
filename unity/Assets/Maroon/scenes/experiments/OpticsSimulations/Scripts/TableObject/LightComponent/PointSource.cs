using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class PointSource : LightComponent
    {
        public int numberOfRays = 40;
        
        private void Start()
        {
            LightRoutes = new List<LightRoute>();
            Origin = transform.localPosition;

            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                    LightRoutes.Add(new LightRoute(wl));

            RecalculateLightRoute();
        }

        public void ChangeNumberOfRays(int nrRays)
        {
            numberOfRays = nrRays;
            
            ResetLightRoutes(numberOfRays);
            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                    LightRoutes.Add(new LightRoute(wl));
            
            RecalculateLightRoute();
        }

        public override void RecalculateLightRoute()
        {
            if (LightRoutes == null)
                return;
            ResetLightRoutes(numberOfRays);

            float angle = 0;
            float angleDelta = 360f / numberOfRays;

            int pos = 0;
            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                {
                    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.right;
                    var initialRay = new RaySegment(Origin, Intensity, wl, transform.rotation * dir);
                    LightRoutes[pos].AddRaySegment(initialRay);
                    LightRoutes[pos].CalculateNextRay(initialRay);
                    pos++;
                    angle += angleDelta;
                }
            
        // public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        // {
        //     throw new NotImplementedException("CheckHitComponent Method not implemented for PointSource!");
        //     // return false;
        // }
            
        }
        
        
    }
}