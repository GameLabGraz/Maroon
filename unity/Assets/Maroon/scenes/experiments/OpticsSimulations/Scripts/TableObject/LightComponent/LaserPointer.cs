using System.Collections.Generic;
using Maroon.Physics.Optics.Light;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.LightComponent
{
    public class LaserPointer : LightComponent
    {
        private void Start()
        {
            LightRoutes = new List<LightPath>();
            Origin = transform.localPosition;

            foreach (var wl in Wavelengths)
                LightRoutes.Add(new LightPath(wl));
            
            RecalculateLightRoute();
        }

        public void SetParameters(LaserPointerParameters parameters)
        {
            // No parameters, do nothing
        }

        public override void RecalculateLightRoute()
        {
            if (LightRoutes == null)
                return;
            
            ResetLightRoutes(1);

            for (int i = 0; i < Wavelengths.Count; i++) // We got Wavelengths.Count many LightRoutes
            {
                var initialRay = new RaySegment(Origin, Intensity, Wavelengths[i], transform.right);
                LightRoutes[i].AddRaySegment(initialRay);
                LightRoutes[i].CalculateNextRay(initialRay);
            }
        }
        
    }
}