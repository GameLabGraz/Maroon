using System.Collections.Generic;
using Maroon.Physics.Optics.Light;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.LightComponent
{
    public class PointSource : LightComponent
    {
        public int numberOfRays = 16;
        public float rayDistributionAngle = 30;
        
        private void Start()
        {
            LightRoutes = new List<LightPath>();
            Origin = transform.localPosition;

            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                    LightRoutes.Add(new LightPath(wl));

            ChangeNumberOfRaysAndAngle(numberOfRays, rayDistributionAngle);
        }
        
        public void SetParameters(PointSourceParameters parameters)
        {
            this.numberOfRays = parameters.numberOfRays;
            this.rayDistributionAngle = parameters.rayDistributionAngle;
            ChangeNumberOfRaysAndAngle(numberOfRays, rayDistributionAngle);
            Component.GetComponent<MeshRenderer>().enabled = parameters.enableMeshRenderer;
        }

        public void ChangeNumberOfRaysAndAngle(int nrRays, float distAngle)
        {
            numberOfRays = nrRays;
            rayDistributionAngle = distAngle;
            if (LightRoutes == null)
                return;
            ResetLightRoutes(numberOfRays);
            
            foreach (var wl in Wavelengths)
                for (int i = 0; i < numberOfRays; i++)
                    LightRoutes.Add(new LightPath(wl));
            
            RecalculateLightRoute();
        }

        public override void RecalculateLightRoute()
        {
            if (LightRoutes == null)
                return;
            ResetLightRoutes(numberOfRays);

            float angle = -rayDistributionAngle / 2;
            float angleDelta = rayDistributionAngle / (numberOfRays - 1);
            
            int pos = 0;
            foreach (var wl in Wavelengths)
            {
                for (int i = 0; i < numberOfRays; i++)
                {
                    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.right;
                    var initialRay = new RaySegment(Origin, Intensity, wl, transform.rotation * dir);
                    LightRoutes[pos].AddRaySegment(initialRay);
                    LightRoutes[pos].CalculateNextRay(initialRay);
                    pos++;
                    angle += angleDelta;
                }
                angle = -rayDistributionAngle / 2;
            }
        }
    }
}