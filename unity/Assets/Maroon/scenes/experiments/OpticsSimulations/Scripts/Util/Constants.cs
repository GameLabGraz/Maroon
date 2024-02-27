using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Util
{
    public static class Constants
    {
        // ----------------------------------- Ray ----------------------------------- //
        public const float BaseRayThicknessInMM = 1.2f;
        public const float ReflectIntensity = 0.1f;
        public const float MinimalIntensity = 0.02f;
        public const int MaxNumberOfRays = 500;

        // ----------------------------------- Camera ----------------------------------- //
        public const float BaseCamFOV = 30;
        private const float BaseCamX = -0.065f;
        private const float BaseCamY = 2.6f;
        private const float BaseCamZ = 1;
        public static readonly Vector3 BaseCamPos = new(BaseCamX, BaseCamY, BaseCamZ);
        public static readonly Quaternion BaseCamRot = Quaternion.Euler(52.5f, 0, 0);
        
        public const float TopCamFOV = 30;
        private const float TopCamX = -0.065f;
        private const float TopCamY = 3.0f;
        private const float TopCamZ = 2.5f;
        public static readonly Vector3 TopCamPos = new(TopCamX, TopCamY, TopCamZ);
        public static readonly Quaternion TopCamRot = Quaternion.Euler(90, 0, 0);
        
        public const float MinFOV = 1f;
        public const float MaxFOV = 70f;
        public static readonly Vector3 MinPositionCamera = new(-2.0f, 1.4f, 0.5f);
        public static readonly Vector3 MaxPositionCamera = new(2.0f, 3.0f, 3.5f);
        
        // ----------------------------------- Optical Table ----------------------------------- //
        public const float Aenv = 1f;   // A and B define the index of refraction of the environment around the lenses 
        public const float Benv = 0f;   // A = 1, B = 0, corresponds to air/vacuum

        public static readonly Vector3 MinPositionTable = new(0f, -0.19f, 0f);
        public static readonly Vector3 MaxPositionTable = new(4f, 1f, 2f);
        public static readonly Vector3 TableBaseOffset = new(-2f, 1f, 1.5f);
        public const float TableHeight = 1f;
        public const float TableObjectHeight = 0.20f;
        public const int TableObjectLayer = 1 << 4;
        public const int UILayer = 1 << 5;
        
        public static readonly Vector3 MirrorTransformArrowPos = new(0.2408f, 0, 0);
        public static readonly Vector3 MirrorArrowShift = new(0.048f, 0, 0);
        
        public static readonly Vector3 BaseOcPosition = new (2.1f, 0, 0.9f);
        public static readonly Vector3 BaseLcPosition = new (1.3f, 0, 0.9f);
        
        // ----------------------------------- Number Conversion ----------------------------------- //
        public const float Epsilon = 0.000001f;
        public const float InCM = 100f;
        public const float InMM = 1000f;

        // ----------------------------------- Tags ----------------------------------- //
        public const string TagRotationArrowY = "rotationY";
        public const string TagRotationArrowZ = "rotationZ";
        public const string TagTranslationArrowY = "translationY";
        
        // ----------------------------------- Lens Presets ----------------------------------- //
        public static readonly (float, float) DenseFlintGlassSF10 = (1.728f, 13420);
        public static readonly (float, float) FusedSilica = (1.458f, 3540);
        public static readonly (float, float) BorosilicateGlassBK7 = (1.5046f, 4200);
        public static readonly (float, float) HardCrownGlassK5 = (1.522f, 4590);
        public static readonly (float, float) BariumCrownGlassBaK4 = (1.569f, 5310);
        public static readonly (float, float) BariumFlintGlassBaF10 = (1.67f, 7430);

        public const float LensPrestRc = 0.065f;
        public static readonly (float, float, float, float) Biconvex = (0.15f, -0.15f, 0.0145f, 0.0145f); 
        public static readonly (float, float, float, float) Planoconvex = (0.15f, -0.50f, 0.015f, 0.006f); 
        public static readonly (float, float, float, float) PositiveMeniscus = (0.12f, 0.21f, 0.02f, 0.02f); 
        public static readonly (float, float, float, float) NegativeMeniscus = (0.21f, 0.12f, 0.02f, 0.02f); 
        public static readonly (float, float, float, float) Planoconcave = (0.50f, 0.10f, 0.01f, 0.01f); 
        public static readonly (float, float, float, float) Biconcave = (-0.10f, 0.10f, 0.01f, 0.01f); 
        public static readonly (float, float, float, float) Ball = (0.065f, -0.065f, 0.065f, 0.065f); 
        
    }
}