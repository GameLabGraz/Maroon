using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseTypes
{
    public class NoiseType2D : NoiseType
    {
        private Noise noise;


        public override bool GenerateNoiseMap(Noise noise)
        {
            this.noise = noise;
            return true;
        }

        public override bool GetNoiseMapValue(Vector3 index) =>
            GetNoise(index[0] * NoiseExperiment.Instance.scale, index[2] * NoiseExperiment.Instance.scale) *
            NoiseExperiment.Instance.size > index[1];

        public override bool GetNoiseMapValue(Vector3Int index) =>
            GetNoise(index[0] * NoiseExperiment.Instance.scale, index[2] * NoiseExperiment.Instance.scale) *
            NoiseExperiment.Instance.size > index[1];

        public override float GetNoiseMapValue(Vector2 index) => GetNoise(index[0], index[1]);

        public override float GetNoiseMapValue(Vector2Int index) => GetNoise(index[0], index[1]);

        private float GetNoise(float x, float y) =>
            noise.GetNoise3D(x, y, NoiseExperiment.Instance.time, NoiseExperiment.Instance.octaves);
    }
}
