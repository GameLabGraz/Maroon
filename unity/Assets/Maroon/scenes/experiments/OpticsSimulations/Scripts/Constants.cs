using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts
{
    public static class Constants
    {
        public const float LaserWidth = 0.003f;
        public const int MouseColliderMaskIndex = 11;
        
        public static Vector3 MinPositionCamera = new Vector3(-2.0f, 1.4f, 0.5f);
        public static Vector3 MaxPositionCamera = new Vector3(2.0f, 3.0f, 3.5f);

        public static Vector3 MinPositionTable = new Vector3(0f, 0f, 0f);
        public static Vector3 MaxPositionTable = new Vector3(4f, 0f, 2f);
        public const float TableHeight = 1f;
    }
}