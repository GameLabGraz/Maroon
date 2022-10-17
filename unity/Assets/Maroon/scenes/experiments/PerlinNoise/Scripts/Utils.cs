using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public static class Utils
    {
        public static Vector2 half_vector_2 => new Vector2(0.5f, 0.5f);
        public static Vector3 half_vector_3 => new Vector3(0.5f, 0.5f, 0.5f);

        public static float Clamp01(this float f) => Clamp(f, 0, 1);

        // clamps f to be 0 < f < 1
        public static float Clamp01Exclusive(this float f) => Clamp(f, float.Epsilon, 1f - 1e-7f);

        public static float Clamp(this float f, float min, float max) => Mathf.Max(Mathf.Min(f, max), min);
        public static int Clamp(this int f, int min, int max) => Mathf.Max(Mathf.Min(f, max), min);

        public static bool IsValidIndex<T>(this T[] list, int index) => index >= 0 && index < list.Length;
        public static bool IsValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

        public static float Map(this float value, float min_source, float max_source, float min_target = 0,
            float max_target = 1)
        {
            return (value - min_source) / (max_source - min_source) * (max_target - min_target) + min_target;
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
