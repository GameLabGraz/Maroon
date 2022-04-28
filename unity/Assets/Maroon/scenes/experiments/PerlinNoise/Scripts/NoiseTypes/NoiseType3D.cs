using System.Linq;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseTypes
{
    public class NoiseType3D : NoiseType
    {
        [SerializeField] public QuantityFloat flatness;

        [SerializeField] private Vector2 threshold_3d_range;
        [SerializeField] private Vector2 threshold_2d_range;

        private Vector3 offset;

        private bool[] noise_map_array;

        private int size;

        public override bool GenerateNoiseMap(Noise noise)
        {
            var dirty = false;
            size = NoiseExperiment.Instance.size;
            if (noise_map_array?.Length != size * size * size)
            {
                noise_map_array = new bool[size * size * size];
                dirty = true;
            }

            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < size; voxel.z++)
                    {
                        var coordinates01 = (Vector3) voxel / (size - 1) - Utils.half_vector_3;
                        var center = offset + Utils.half_vector_3;
                        var noise_pos = center + coordinates01 * NoiseExperiment.Instance.scale;
                        var n3 = noise.GetNoise3D(noise_pos.x, noise_pos.y, noise_pos.z,
                            NoiseExperiment.Instance.octaves);
                        var n2 = noise.GetNoise2D(noise_pos.x, noise_pos.z, NoiseExperiment.Instance.octaves);
                        n3 = n3.Map(threshold_3d_range.x, threshold_3d_range.y);
                        n2 = n2.Map(threshold_2d_range.x, threshold_2d_range.y);
                        var n = n3 * (1 - flatness) + (n2 - (float) voxel.y / size) * flatness;
                        n = BorderAdjustment(voxel, n);
                        var fill = n > 0;

                        if (GetNoiseMapArray(voxel) != fill)
                            dirty = true;
                        SetNoiseMapArray(voxel, fill);
                    }
                }
            }

            return dirty;
        }

        private float BorderAdjustment(Vector3Int voxel, float n)
        {
            var min_value = Mathf.Min(voxel.x, voxel.y, voxel.z);
            var max_value = Mathf.Max(voxel.x, voxel.y, voxel.z);
            min_value = Mathf.Min(min_value, Mathf.Abs(max_value - size));

            return n - Mathf.Max(1 / (min_value + 0.3f) - 0.2f, 0);
        }

        private void SetNoiseMapArray(int x, int y, int z, bool value) =>
            noise_map_array[x * size * size + y * size + z] = value;

        private void SetNoiseMapArray(Vector3Int index, bool value) =>
            SetNoiseMapArray(index.x, index.y, index.z, value);

        private bool GetNoiseMapArray(int x, int y, int z) =>
            IsValidNoiseArrayIndex(x, y, z) && noise_map_array[x * size * size + y * size + z];

        private bool GetNoiseMapArray(Vector3Int index) => GetNoiseMapArray(index.x, index.y, index.z);

        private bool IsValidNoiseArrayIndex(int x, int y, int z) =>
            x.IsInRange(0, size - 1) && y.IsInRange(0, size - 1) && z.IsInRange(0, size - 1);


        public override bool GetNoiseMapValue(Vector3 index)
        {
            return GetNoiseMapArray((int) index[0], (int) index[1], (int) index[2]);
        }

        public override bool GetNoiseMapValue(Vector3Int index)
        {
            return GetNoiseMapArray(index);
        }

        public override float GetNoiseMapValue(Vector2 index)
        {
            return GetNoiseMapArray((int) index[0], 0, (int) index[1]) ? 1 : -1;
        }

        public override float GetNoiseMapValue(Vector2Int index)
        {
            return GetNoiseMapArray(index[0], 0, index[1]) ? 1 : -1;
        }
    }
}
