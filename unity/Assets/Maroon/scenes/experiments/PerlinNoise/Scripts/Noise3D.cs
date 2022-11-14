using System;
using System.Linq;
using Maroon.Physics;
using Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class Noise3D : MonoBehaviour
    {
        private int size;
        private Vector3 offset;

        private float[] noise_map_array;

        [SerializeField] public QuantityFloat flatness;
        [SerializeField] public QuantityFloat threshold;

        private bool parameters_dirty = false;

        public float max;
        public float min;

        public bool GenerateNoiseMap()
        {
            var dirty = false;
            size = NoiseExperiment.Instance.size;
            if (noise_map_array?.Length != size * size * size)
            {
                noise_map_array = new float[size * size * size];
                dirty = true;
            }

            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < size; voxel.z++)
                    {
                        var coordinates01 = (Vector3)voxel / (size - 1) - Utils.half_vector_3;
                        var center = offset + Utils.half_vector_3;
                        var noise_pos = center + coordinates01 * ((NoiseExperiment.Instance.scale + 1) * 0.3f);
                        var n3 = NoiseExperiment.Noise3D.GetNoise3D(noise_pos.x, noise_pos.y, noise_pos.z,
                            NoiseExperiment.Instance.octaves);
                        var n2 = NoiseExperiment.Noise3D.GetNoise2D(noise_pos.x, noise_pos.z,
                            NoiseExperiment.Instance.octaves);
                        n2 = n2.Map(-2, 2);
                        var n = n3 * (1 - flatness) + (n2 - (float)voxel.y / size) * flatness;
                        n = BorderAdjustment(voxel, n);
                        n = n.LogSigmoid();

                        if (NoiseVisualisationMarchingCubes.CheckN(voxel, out var f))
                            n = f;

                        if (!dirty && Math.Abs(GetNoiseMapArrayF(voxel) - n) > 0.001f)
                            dirty = true;
                        SetNoiseMapArray(voxel, n);
                    }
                }
            }

            max = noise_map_array.Max();
            min = noise_map_array.Min();
            min = Mathf.Lerp(min, max, 0.4f);
            noise_map_array = noise_map_array.Select(n => n.Map(min, max)).ToArray();

            var parameter_dirty_tmp = parameters_dirty;
            parameters_dirty = false;

            return dirty || parameter_dirty_tmp;
        }


        private float BorderAdjustment(Vector3Int voxel, float n)
        {
            var min_value = Mathf.Min(voxel.x, voxel.y, voxel.z);
            var max_value = Mathf.Max(voxel.x, voxel.y, voxel.z);
            min_value = Mathf.Min(min_value, Mathf.Abs(max_value - size));

            return n - Mathf.Max(1 / (min_value + 0.3f) - 0.2f, 0);
        }


        public void SetNoiseMapArray(int x, int y, int z, float value) =>
            noise_map_array[x * size * size + y * size + z] = value;

        public void SetNoiseMapArray(Vector3Int index, float value) =>
            SetNoiseMapArray(index.x, index.y, index.z, value);

        public float GetNoiseMapArrayF(int x, int y, int z)
        {
            if (!IsValidNoiseArrayIndex(x, y, z))
                return float.MinValue;
            return noise_map_array[x * size * size + y * size + z];
        }

        public float GetNoiseMapArrayF(Vector3Int index) => GetNoiseMapArrayF(index.x, index.y, index.z);

        public bool GetNoiseMapArrayB(Vector3Int index) => GetNoiseMapArrayF(index.x, index.y, index.z) > threshold;

        public bool GetNoiseMapArrayB(int x, int y, int z) => GetNoiseMapArrayF(x, y, z) > threshold;

        public bool IsValidNoiseArrayIndex(int x, int y, int z) =>
            x.IsInRange(0, size - 1) && y.IsInRange(0, size - 1) && z.IsInRange(0, size - 1);

        private void Awake()
        {
            Init();
        }

        private void OnValidate()
        {
            Init();
        }

        private void Init()
        {
            flatness.onValueChanged.RemoveAllListeners();
            flatness.onValueChanged.AddListener(_ =>
            {
                parameters_dirty = true;
                NoiseExperiment.Instance.SetDirty();
            });
            threshold.onValueChanged.RemoveAllListeners();
            threshold.onValueChanged.AddListener(_ =>
            {
                parameters_dirty = true;
                NoiseExperiment.Instance.SetDirty();
            });
        }
    }
}