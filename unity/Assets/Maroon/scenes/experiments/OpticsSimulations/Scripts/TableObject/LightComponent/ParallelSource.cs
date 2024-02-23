using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class ParallelSource : LightComponent
    {
        public int numberOfRays = 16;
        public float distanceBetweenRays = 0.38f / Constants.InMM;

        private void Start()
        {
            LightRoutes = new List<LightRoute>();
            Origin = transform.localPosition;

            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                    LightRoutes.Add(new LightRoute(wl));

            ChangeNumberOfRays(numberOfRays);
        }
        
        public void SetParameters(int numberOfRays = 16, float distanceBetweenRays = 0.00015f)
        {
            this.distanceBetweenRays = distanceBetweenRays;
            ChangeNumberOfRays(numberOfRays);
        }

        public void ChangeNumberOfRays(int nrRays)
        {
            numberOfRays = nrRays;
            if (LightRoutes == null)
                return;
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
        

        private List<Vector3> CalculateRayPositions()
        {
            List<Vector3> rayPositions = new List<Vector3>();
            int half = numberOfRays / 2;
            
            // odd
            if (numberOfRays % 2 == 1)
                for (int i = -half; i <= half; i++)
                    rayPositions.Add(Origin + transform.forward * (i * distanceBetweenRays));
            // even
            else
                for (int i = -numberOfRays + 1; i <= numberOfRays - 1; i += 2)
                    rayPositions.Add(Origin + transform.forward * (i * (distanceBetweenRays / 2)));

            return rayPositions;
        }
        
    }
}