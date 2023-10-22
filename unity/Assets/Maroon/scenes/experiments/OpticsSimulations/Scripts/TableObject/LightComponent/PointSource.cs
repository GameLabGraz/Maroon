using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class PointSource : LightComponent
    {
        private int nrOfLights;
        private LightRoute[] _lightRoutes;

        public PointSource(int nrOfLights)
        {
            this.nrOfLights = nrOfLights;
        }

        private void Start()
        {
            _lightRoutes = new LightRoute[nrOfLights];
            for (int i = 0; i < nrOfLights; i++)
                _lightRoutes[i] = new LightRoute(Intensity, Wavelength);
        }
        
        
        public override bool CheckHitComponent(OpticalComponent.OpticalComponent oc)
        {
            throw new NotImplementedException("CheckHitComponent Method not implemented for PointSource!");
            // return false;
        }
        
        public override void RecalculateLightRoute()
        {
            throw new NotImplementedException("RecalculateLightRoute Method not implemented for PointSource!");
        }
        
    }
}