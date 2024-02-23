using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class LaserPointer : LightComponent
    {
        private void Start()
        {
            LightRoutes = new List<LightRoute>();
            Origin = transform.localPosition;

            foreach (var wl in Wavelengths)
                LightRoutes.Add(new LightRoute(wl));
            
            RecalculateLightRoute();
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