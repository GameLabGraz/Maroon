using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public static class Utils
    {
        public static Vector2 HalfVector2 => new Vector2(0.5f, 0.5f);
        public static Vector3 HalfVector3 => new Vector3(0.5f, 0.5f, 0.5f);

        public static float Clamp(this float f, float min, float max) => Mathf.Max(Mathf.Min(f, max), min);
        public static int Clamp(this int f, int min, int max) => Mathf.Max(Mathf.Min(f, max), min);

        public static bool IsValidIndex<T>(this T[] list, int index) => index >= 0 && index < list.Length;
        public static bool IsValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

        public static float Map(this float value, float minSource, float maxSource, float minTarget = 0,
            float maxTarget = 1)
        {
            return (value - minSource) / (maxSource - minSource) * (maxTarget - minTarget) + minTarget;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this int value, int min, int max) => value >= min && value <= max;


        //runs code at the end of the frame
        public static void EndFrame(this MonoBehaviour mb, Action action)
        {
            mb.StartCoroutine(_EndFrame(action));
        }


        private static IEnumerator _EndFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        public static float LogSigmoid(this float x)
        {
            if (x < -45.0) return 0.0f;
            if (x > 45.0) return 1.0f;
            return 1.0f / (1.0f + Mathf.Exp(-x));
        }
    }
}
