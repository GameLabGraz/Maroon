using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class ParallelSource : LightComponent
    {
        public int numberOfRays = 20;
        public float distanceBetweenRays = Constants.LaserWidth * 3;

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
            
            List<Vector3> rayPositions = CalculateRayPositions();
            
            int pos = 0;
            foreach (var wl in Wavelengths)
                foreach (var rp in rayPositions)
                {
                    LightRoutes[pos].ResetLightRoute();
                    var initialRay = new RaySegment(rp, Intensity, wl, transform.right);
                    LightRoutes[pos].AddRaySegment(initialRay);
                    LightRoutes[pos].CalculateNextRay(initialRay);
                    pos++;
                }
        }
        

        // todo den origin der rays richtig berechnen für rotation
        
        private List<Vector3> CalculateRayPositions()
        {
            List<Vector3> rayPositions = new List<Vector3>();
            int half = numberOfRays / 2;
            // odd
            if (numberOfRays % 2 == 1)
                for (int i = -half; i <= half; i++)
                {
                    var test = Origin + transform.forward * (i * distanceBetweenRays);
                    rayPositions.Add(test);
                }
            // even
            else
                for (int i = -numberOfRays + 1; i <= numberOfRays - 1; i += 2)
                {
                    var test = Origin + transform.forward * (i * (distanceBetweenRays / 2));
                    rayPositions.Add(test);
                }

            return rayPositions;
        }
        
        // public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        // {
        //     return oc == OpticalComponentManager.Instance.GetFirstHitComponent(Origin, transform.right);
        // }

        
    }
}