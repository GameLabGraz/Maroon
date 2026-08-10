using System;
using System.Linq;
using Maroon.Physics;
using Maroon.ComputerScience.PerlinNoise.NoiseVisualisations;
using Maroon.Utils;
using UnityEngine;

namespace Maroon.ComputerScience.PerlinNoise
{
    public class Noise3D : MonoBehaviour
    {
        [SerializeField] public QuantityFloat threshold;
        [SerializeField] private float max;
        [SerializeField] private float min;
        
        private int _size;
        private readonly Vector3 _offset;
        private float[] _noise_map_array;
        private bool _parameters_dirty;

        public Noise3D(Vector3 offset)
        {
            _offset = offset;
        }

        public bool GenerateNoiseMap()
        {
            var dirty = false;
            _size = NoiseExperimentBase.Instance.size;
            if (_noise_map_array?.Length != _size * _size * _size)
            {
                _noise_map_array = new float[_size * _size * _size];
                dirty = true;
            }

            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < _size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < _size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < _size; voxel.z++)
                    {
                        var coordinates01 = (Vector3)voxel / (_size - 1) - Maroon.Utils.Utils.HalfVector3;
                        var center = _offset + Maroon.Utils.Utils.HalfVector3;
                        var noisePos = center + coordinates01 * ((
                            NoiseExperimentBase.Instance.scale + 1) * 0.3f);
                        var n = NoiseExperimentBase.noise.GetNoise3D(noisePos, NoiseExperimentBase.Instance.octaves);
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

            max = _noise_map_array.Max();
            min = _noise_map_array.Min();
            min = Mathf.Lerp(min, max, 0.4f);
            _noise_map_array = _noise_map_array.Select(n => n.Map(min, max)).ToArray();

            var parametersDirty = _parameters_dirty;
            _parameters_dirty = false;

            return dirty || parametersDirty;
        }


        private float BorderAdjustment(Vector3Int voxel, float n)
        {
            var minValue = Mathf.Min(voxel.x, voxel.y, voxel.z);
            var maxValue = Mathf.Max(voxel.x, voxel.y, voxel.z);
            minValue = Mathf.Min(minValue, Mathf.Abs(maxValue - _size));

            return n - Mathf.Max(1 / (minValue + 0.3f) - 0.2f, 0);
        }


        private void SetNoiseMapArray(int x, int y, int z, float value) =>
            _noise_map_array[x * _size * _size + y * _size + z] = value;

        private void SetNoiseMapArray(Vector3Int index, float value) =>
            SetNoiseMapArray(index.x, index.y, index.z, value);

        private float GetNoiseMapArrayF(int x, int y, int z)
        {
            if (!IsValidNoiseArrayIndex(x, y, z))
                return float.MinValue;
            return _noise_map_array[x * _size * _size + y * _size + z];
        }

        public float GetNoiseMapArrayF(Vector3Int index) => GetNoiseMapArrayF(index.x, index.y, index.z);

        public bool GetNoiseMapArrayB(Vector3Int index) => GetNoiseMapArrayF(index.x, index.y, index.z) > threshold;

        public bool GetNoiseMapArrayB(int x, int y, int z) => GetNoiseMapArrayF(x, y, z) > threshold;

        private bool IsValidNoiseArrayIndex(int x, int y, int z) =>
            x.IsInRange(0, _size - 1) && y.IsInRange(0, _size - 1) && z.IsInRange(0, _size - 1);

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
            threshold.onValueChanged.RemoveAllListeners();
            threshold.onValueChanged.AddListener(_ =>
            {
                _parameters_dirty = true;
                NoiseExperimentBase.Instance.SetDirty();
            });
        }
    }
}