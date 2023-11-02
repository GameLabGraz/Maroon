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
        [Header("Parallel Source Settings")] 
        private List<LightRoute> _lightRoutes;

        public int numberOfRays = 4;
        public float distanceBetweenRays = Constants.LaserWidth * 3;

        private void Start()
        {
            _lightRoutes = new List<LightRoute>();
            Origin = transform.localPosition;

            for (int i = 0; i < numberOfRays; i++)
                _lightRoutes.Add(new LightRoute(Wavelength));

            RecalculateLightRoute();
        }
        
        public override void RecalculateLightRoute()
        {
            List<Vector3> rayPositions = CalculateRayPositions();

            for (int i = 0; i < numberOfRays; i++)
            {
                _lightRoutes[i].ResetLightRoute();
                
                var initialRay = new RaySegment(rayPositions[i], Intensity, Wavelength, transform.right);
                _lightRoutes[i].AddRaySegment(initialRay);
                _lightRoutes[i].CalculateNextRay(initialRay);
            }
            
        }

        private List<Vector3> CalculateRayPositions()
        {
            List<Vector3> rayPositions = new List<Vector3>();
            int half = numberOfRays / 2;
            // odd
            if (numberOfRays % 2 == 1)
                for (int i = -half; i <= half; i++)
                    rayPositions.Add(new Vector3(Origin.x, Origin.y, Origin.z + i * distanceBetweenRays));
            // even
            else
                for (int i = -numberOfRays + 1; i <= numberOfRays - 1; i += 2)
                    rayPositions.Add(new Vector3(Origin.x, Origin.y, Origin.z + i * (distanceBetweenRays / 2)));

            return rayPositions;
        }
        
        public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        {
            return oc == OpticalComponentManager.Instance.GetFirstHitComponent(Origin, transform.right);
        }

        
    }
}