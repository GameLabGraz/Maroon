﻿using UnityEngine;

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
        
        public static Vector3 TableBaseOffset = new Vector3(-2f, 1f, 1.5f);
        public const float TableHeight = 1f;
        public const float TableObjectHeight = 0.25f;
        public const float Epsilon = 0.000001f;
    }
}