namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class Noise : OpenSimplexNoise
    {
        public Noise(int seed, float offset = 0) : base(seed)
        {
            this.offset = offset;
        }

        public float offset;

        public float GetNoise2D(float x, float y, float octaves)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += Evaluate(x * i, y * i, offset) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += Evaluate(x * i, y * i, offset) * last_octave_fraction / i;

            return (float)noise;
        }

        public float GetNoise3D(float x, float y, float z, float octaves)
        {
            var noise = 0.0;
            int i;
            for (i = 1; i < octaves - float.Epsilon; i++)
                noise += Evaluate(x * i, y * i, z * i, offset) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += Evaluate(x * i, y * i, z * i, offset) * last_octave_fraction / i;

            return (float)noise;
        }
    }
}