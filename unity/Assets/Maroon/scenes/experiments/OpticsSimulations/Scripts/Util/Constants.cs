﻿using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts
{
    public static class Constants
    {
        public const float LaserWidth = 0.003f;
        public const float ReflectIntensity = 0.1f;
        public const float MinimalIntensity = 0.02f;
        public const int MaxNumberOfRays = 500;
        public const float Aenv = 1f;   // A and B define the index of refraction of the environment around the lenses 
        public const float Benv = 0f;   // A = 1, B = 0, corresponds to air/vacuum

        public static Vector3 MinPositionCamera = new Vector3(-2.0f, 1.4f, 0.5f);
        public static Vector3 MaxPositionCamera = new Vector3(2.0f, 3.0f, 3.5f);

        public static Vector3 MinPositionTable = new Vector3(0f, 0f, 0f);
        public static Vector3 MaxPositionTable = new Vector3(4f, 0f, 2f);
        
        public static Vector3 TableBaseOffset = new Vector3(-2f, 1f, 1.5f);
        public const float TableHeight = 1f;
        public const float TableObjectHeight = 0.20f;
        public const float Epsilon = 0.000001f;
    }
}