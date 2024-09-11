using UnityEngine;

namespace Maroon.ComputerScience.PerlinNoise
{
    public class Noise : OpenSimplexNoise
    {
        public float offset;

        public Noise(int seed, float offset = 0) : base(seed)
        {
            this.offset = offset;
        }

        public float GetNoise2D(Vector2 pos, float octaves = 1) => GetNoise2D(pos.x, pos.y, octaves);

        public float GetNoise2D(float x, float y, float octaves = 1)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += Evaluate(x * i + i * 100, y * i + i * 100, offset) / i;
            var lastOctaveFraction = octaves - i + 1;
            noise += Evaluate(x * i + i * 100, y * i + i * 100, offset) * lastOctaveFraction / i;

            return (float)noise;
        }

        public float GetNoise3D(Vector3 pos, float octaves = 1) => GetNoise3D(pos.x, pos.y, pos.z, octaves);

        public float GetNoise3D(float x, float y, float z, float octaves = 1)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += Evaluate(x * i + i * 100, y * i + i * 100, z * i + i * 100, offset) / i;
            var lastOctaveFraction = octaves - i + 1;
            noise += Evaluate(x * i + i * 100, y * i + i * 100, z * i + i * 100, offset) * lastOctaveFraction / i;

            return (float)noise;
        }
    }
}