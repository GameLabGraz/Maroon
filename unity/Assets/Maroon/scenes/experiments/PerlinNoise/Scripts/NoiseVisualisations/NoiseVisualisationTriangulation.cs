using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations
{
    public class NoiseVisualisationTriangulation : NoiseVisualisation
    {
        [SerializeField] [Range(0, 5)] private float heightScale = 1;
        [SerializeField] [Range(0, 0.2f)] private float thickness = 1;
        private int _noise_samples;
        private int _size;
        private float _total_noise_height;
        private List<Vector3> _vertices;

        public override void GenerateMesh(Mesh mesh)
        {
            _size = NoiseExperiment.Instance.size;
            _noise_samples = 0;
            _total_noise_height = 0;

            var vertexCount = _size * _size * 2 + _size * 8;
            _vertices = new List<Vector3>(vertexCount);
            var UVs = new List<Vector2>(vertexCount);
            var indices = new List<int>();

            for (var x = 0; x < _size; x++)
            {
                for (var y = 0; y < _size; y++)
                {
                    _vertices.Add(GetVertexNoise(x, y));
                    UVs.Add(new Vector2(x, y));

                    if (x == 0 || y == 0)
                        continue;

                    indices.AddRange(new[]
                    {
                        _vertices.Count - 1, _vertices.Count - 2, _vertices.Count - 1 - _size,
                        _vertices.Count - 2 - _size, _vertices.Count - 1 - _size, _vertices.Count - 2
                    });
                }
            }

            for (var x = 0; x < _size; x++)
            {
                for (var y = 0; y < _size; y++)
                {
                    _vertices.Add(_vertices[x * _size + y] + Vector3.down * thickness);
                    UVs.Add(new Vector2(x, y));

                    if (x == 0 || y == 0)
                        continue;

                    indices.AddRange(new[]
                    {
                        _vertices.Count - 1, _vertices.Count - 1 - _size, _vertices.Count - 2,
                        _vertices.Count - 2 - _size, _vertices.Count - 2, _vertices.Count - 1 - _size
                    });
                }
            }

            for (var x = 0; x < _size; x++) //left skirt
            {
                var n = GetVertexNoise(x, 0);
                _vertices.Add(n + Vector3.down * thickness);
                _vertices.Add(n);
                UVs.Add(new Vector2(x, -1));
                UVs.Add(new Vector2(x, 0));

                if (x == 0)
                    continue;

                indices.AddRange(new[]
                {
                    _vertices.Count - 1, _vertices.Count - 2, _vertices.Count - 3,
                    _vertices.Count - 4, _vertices.Count - 3, _vertices.Count - 2
                });
            }

            for (var y = 0; y < _size; y++) //bottom skirt
            {
                var n = GetVertexNoise(_size - 1, y);
                _vertices.Add(n + Vector3.down * thickness);
                _vertices.Add(n);
                UVs.Add(new Vector2(_size, y));
                UVs.Add(new Vector2(_size - 1, y));

                if (y == 0)
                    continue;

                indices.AddRange(new[]
                {
                    _vertices.Count - 1, _vertices.Count - 2, _vertices.Count - 3,
                    _vertices.Count - 4, _vertices.Count - 3, _vertices.Count - 2
                });
            }

            for (var x = 0; x < _size; x++) //right skirt
            {
                var n = GetVertexNoise(x, _size - 1);
                _vertices.Add(n + Vector3.down * thickness);
                _vertices.Add(n);
                UVs.Add(new Vector2(x, _size));
                UVs.Add(new Vector2(x, _size - 1));

                if (x == 0)
                    continue;

                indices.AddRange(new[]
                {
                    _vertices.Count - 1, _vertices.Count - 3, _vertices.Count - 2,
                    _vertices.Count - 4, _vertices.Count - 2, _vertices.Count - 3
                });
            }

            for (var y = 0; y < _size; y++) //top skirt
            {
                var n = GetVertexNoise(0, y);
                _vertices.Add(n + Vector3.down * thickness);
                _vertices.Add(n);
                UVs.Add(new Vector2(-1, y));
                UVs.Add(new Vector2(0, y));

                if (y == 0)
                    continue;

                indices.AddRange(new[]
                {
                    _vertices.Count - 1, _vertices.Count - 3, _vertices.Count - 2,
                    _vertices.Count - 4, _vertices.Count - 2, _vertices.Count - 3
                });
            }

            if (!mesh)
                return;

            mesh.Clear();

            mesh.vertices = _vertices.Select(v => v - new Vector3(0, _total_noise_height / _noise_samples, 0))
                .ToArray();
            mesh.uv = UVs.ToArray();
            mesh.colors = _vertices
                .Select(v => NoiseExperiment.Instance.GetVertexColor(v.y.Map(-heightScale, heightScale)))
                .ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            _noise_samples = 0;
            _total_noise_height = 0;
            _size = NoiseExperiment.Instance.size;


            var vertexCount = _size * _size * 2 + _size * 8;
            if (_vertices == null || _vertices.Count != vertexCount)
            {
                GenerateMesh(mesh);
                return;
            }

            var index = _size * _size;
            for (var x = 0; x < _size; x++)
            {
                for (var y = 0; y < _size; y++)
                {
                    _vertices[x * _size + y] = GetVertexNoise(x, y);
                    _vertices[x * _size + y + index] = _vertices[x * _size + y] + Vector3.down * thickness;
                }
            }

            index *= 2;
            for (var x = 0; x < _size; x++) //left skirt
            {
                var n = GetVertexNoise(x, 0);
                _vertices[index + 2 * x] = n + Vector3.down * thickness;
                _vertices[index + 2 * x + 1] = n;
            }

            index += _size * 2;
            for (var y = 0; y < _size; y++) //bottom skirt
            {
                var n = GetVertexNoise(_size - 1, y);
                _vertices[index + 2 * y] = n + Vector3.down * thickness;
                _vertices[index + 2 * y + 1] = n;
            }

            index += _size * 2;
            for (var x = 0; x < _size; x++) //right skirt
            {
                var n = GetVertexNoise(x, _size - 1);
                _vertices[index + 2 * x] = n + Vector3.down * thickness;
                _vertices[index + 2 * x + 1] = n;
            }

            index += _size * 2;
            for (var y = 0; y < _size; y++) //top skirt
            {
                var n = GetVertexNoise(0, y);
                _vertices[index + 2 * y] = n + Vector3.down * thickness;
                _vertices[index + 2 * y + 1] = n;
            }

            if (!mesh)
                return;

            mesh.vertices = _vertices.Select(v => v - new Vector3(0, _total_noise_height / _noise_samples, 0))
                .ToArray();

            mesh.colors = _vertices
                .Select(v => NoiseExperiment.Instance.GetVertexColor(v.y.Map(-heightScale, heightScale)))
                .ToArray();
            mesh.RecalculateNormals();
        }


        private Vector3 GetVertexNoise(float x, float y)
        {
            var coordinates01 = new Vector2(x, y) / (_size - 1) - Utils.HalfVector2;
            var center = Utils.HalfVector2;
            var pos = center + coordinates01 * NoiseExperiment.Instance.scale;
            var height = NoiseExperiment.noise.GetNoise2D(pos, NoiseExperiment.Instance.octaves);
            height *= heightScale;

            _total_noise_height += height;
            _noise_samples++;

            return new Vector3(coordinates01.x, height, coordinates01.y);
        }
    }
}