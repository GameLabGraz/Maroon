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
        private LightRoute _lightRoute;

        public LightRoute LightRoute => _lightRoute;

        private void Start()
        {
            _lightRoute = new LightRoute(Intensity, Wavelength);
        }
        
        private void Update()
        {
            if (transform.hasChanged)
            {
                _lightRoute.ResetLightRoute(Wavelength);
                _lightRoute.CalculateNextRay(transform.localPosition, Vector3.right);
                transform.hasChanged = false;
            }
        }

        // Returns the initial or starting ray from the light source
        public RaySegment GetInitialRay()
        {
            return _lightRoute.GetFirstClearRest();
        }

        
    }
}