using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class LaserPointer : LightComponent
    {
        [Header("Laser Pointer Settings")] 
        private LightRoute _lightRoute;

        public LightRoute LightRoute => _lightRoute;

        private void Start()
        {
            _lightRoute = new LightRoute(Wavelength);
            Origin = transform.localPosition;
            RecalculateLightRoute();
        }
        
        public override void RecalculateLightRoute()
        {
            _lightRoute.ResetLightRoute();

            var initialRay = new RaySegment(Origin, Intensity, Wavelength, transform.right);
            _lightRoute.AddRaySegment(initialRay);
            _lightRoute.CalculateNextRay(initialRay);
        }
    }
}