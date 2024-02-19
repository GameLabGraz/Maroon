﻿using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Util
{
    public static class Constants
    {
        public const float BaseRayThicknessInMM = 1.2f;
        public const float ReflectIntensity = 0.1f;
        public const float MinimalIntensity = 0.02f;
        public const int MaxNumberOfRays = 500;

        public static readonly Vector3 BaseCamPos = new(0, 2.6f, 1);
        public const float BaseCameraX = 0;
        public const float BaseCameraY = 2.6f;
        public const float BaseCameraZ = 1;
        public const float BaseCameraFOV = 60;
        public static readonly Vector3 CamTopPos = new(0, 3f, 2.5f);
        public static readonly Quaternion CamTopRot = Quaternion.Euler(90, 0, 0);
        
        public const float Aenv = 1f;   // A and B define the index of refraction of the environment around the lenses 
        public const float Benv = 0f;   // A = 1, B = 0, corresponds to air/vacuum

        public static readonly Vector3 MinPositionCamera = new(-2.0f, 1.4f, 0.5f);
        public static readonly Vector3 MaxPositionCamera = new(2.0f, 3.0f, 3.5f);
        public static readonly Vector3 MinPositionTable = new(0f, -0.19f, 0f);
        public static readonly Vector3 MaxPositionTable = new(4f, 1f, 2f);
        public static readonly Vector3 TableBaseOffset = new(-2f, 1f, 1.5f);
        public const float TableHeight = 1f;
        public const float TableObjectHeight = 0.20f;
        public const int TableObjectLayer = 1 << 4;
        public const int UILayer = 1 << 5;
        
        public static readonly Vector3 MirrorTransformArrowPos = new(0.2408f, 0, 0);
        public static readonly Vector3 MirrorArrowShift = new(0.048f, 0, 0);
        
        public const float Epsilon = 0.000001f;
        public const float InCM = 100f;
        public const float InMM = 1000f;

        public const string TagRotationArrowY = "rotationY";
        public const string TagRotationArrowZ = "rotationZ";
        public const string TagTranslationArrowY = "translationY";
    }
}