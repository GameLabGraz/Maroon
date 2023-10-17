using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightSource
{
    public class LightSource : TableObject
    {
        [Header("Light Settings")] 
        [SerializeField] private Color color;
        [SerializeField] private float intensity;
        [SerializeField] private float wavelength;
        
        public Color Color
        {
            get => color;
            set => color = value;
        }

        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }

        public float Wavelength
        {
            get => wavelength;
            set => wavelength = value;
        }

        private void RayIntersectionHandler()
        {
            
        }

    }
}
