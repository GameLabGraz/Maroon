using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class Noise2D : NoiseExperiment
    {
        [SerializeField] private QuantityFloat scale;
        [SerializeField] private QuantityFloat octaves;

        [SerializeField, Range(3, 100)] int size = 10;
        [SerializeField, Range(0, 5)] float height_scale = 1;
        [SerializeField, Range(0, 0.2f)] float thickness = 1;

        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        private List<Vector3> vertices;

        private float time;
        private Vector2 offset;


        public override void GenerateMesh(Mesh mesh)
        {
            offset = new Vector2(Random.value, Random.value) * 1e2f;


            var vertex_count = size * size * 2 + size * 8;
            vertices = new List<Vector3>(vertex_count);
            var UVs = new List<Vector2>(vertex_count);
            var indices = new List<int>();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    vertices.Add(GetVertexNoise(x, y));
                    UVs.Add(new Vector2(x, y));

                    if (x == 0 || y == 0)
                        continue;

                    indices.AddRange(new[]
                    {
                        vertices.Count - 1, vertices.Count - 2, vertices.Count - 1 - size,
                        vertices.Count - 2 - size, vertices.Count - 1 - size, vertices.Count - 2
                    });
                }
            }

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    vertices.Add(vertices[x * size + y] + Vector3.down * thickness);
                    UVs.Add(new Vector2(x, y));

                    if (x == 0 || y == 0)
                        continue;

                    indices.AddRange(new[]
                    {
                        vertices.Count - 1, vertices.Count - 1 - size, vertices.Count - 2,
                        vertices.Count - 2 - size, vertices.Count - 2, vertices.Count - 1 - size
                    });
                }
            }

            for (int x = 0; x < size; x++) //left skirt
            {
                var noise = GetVertexNoise(x, 0);
                vertices.Add(noise + Vector3.down * thickness);
                vertices.Add(noise);
                UVs.Add(new Vector2(x, -1));
                UVs.Add(new Vector2(x, 0));

                if (x == 0)
                    continue;

                indices.AddRange(new[]
                {
                    vertices.Count - 1, vertices.Count - 2, vertices.Count - 3,
                    vertices.Count - 4, vertices.Count - 3, vertices.Count - 2
                });
            }

            for (int y = 0; y < size; y++) //bottom skirt
            {
                var noise = GetVertexNoise(size - 1, y);
                vertices.Add(noise + Vector3.down * thickness);
                vertices.Add(noise);
                UVs.Add(new Vector2(size, y));
                UVs.Add(new Vector2(size - 1, y));

                if (y == 0)
                    continue;

                indices.AddRange(new[]
                {
                    vertices.Count - 1, vertices.Count - 2, vertices.Count - 3,
                    vertices.Count - 4, vertices.Count - 3, vertices.Count - 2
                });
            }

            for (int x = 0; x < size; x++) //right skirt
            {
                var noise = GetVertexNoise(x, size - 1);
                vertices.Add(noise + Vector3.down * thickness);
                vertices.Add(noise);
                UVs.Add(new Vector2(x, size));
                UVs.Add(new Vector2(x, size - 1));

                if (x == 0)
                    continue;

                indices.AddRange(new[]
                {
                    vertices.Count - 1, vertices.Count - 3, vertices.Count - 2,
                    vertices.Count - 4, vertices.Count - 2, vertices.Count - 3
                });
            }

            for (int y = 0; y < size; y++) //top skirt
            {
                var noise = GetVertexNoise(0, y);
                vertices.Add(noise + Vector3.down * thickness);
                vertices.Add(noise);
                UVs.Add(new Vector2(-1, y));
                UVs.Add(new Vector2(0, y));

                if (y == 0)
                    continue;

                indices.AddRange(new[]
                {
                    vertices.Count - 1, vertices.Count - 3, vertices.Count - 2,
                    vertices.Count - 4, vertices.Count - 2, vertices.Count - 3
                });
            }

            if (!mesh)
                return;

            mesh.vertices = vertices.ToArray();
            mesh.uv = UVs.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            time += Time.deltaTime * speed;

            var index = size * size;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    vertices[x * size + y] = GetVertexNoise(x, y);
                    vertices[x * size + y + index] = vertices[x * size + y] + Vector3.down * thickness;
                }
            }

            index *= 2;
            for (int x = 0; x < size; x++) //left skirt
            {
                var noise = GetVertexNoise(x, 0);
                vertices[index + 2 * x] = noise + Vector3.down * thickness;
                vertices[index + 2 * x + 1] = noise;
            }

            index += size * 2;
            for (int y = 0; y < size; y++) //bottom skirt
            {
                var noise = GetVertexNoise(size - 1, y);
                vertices[index + 2 * y] = noise + Vector3.down * thickness;
                vertices[index + 2 * y + 1] = noise;
            }

            index += size * 2;
            for (int x = 0; x < size; x++) //right skirt
            {
                var noise = GetVertexNoise(x, size - 1);
                vertices[index + 2 * x] = noise + Vector3.down * thickness;
                vertices[index + 2 * x + 1] = noise;
            }

            index += size * 2;
            for (int y = 0; y < size; y++) //top skirt
            {
                var noise = GetVertexNoise(0, y);
                vertices[index + 2 * y] = noise + Vector3.down * thickness;
                vertices[index + 2 * y + 1] = noise;
            }

            if (!mesh)
                return;

            mesh.vertices = vertices.ToArray();
            mesh.RecalculateNormals();
        }


        private Vector3 GetVertexNoise(float x, float y)
        {
            var coordinates = new Vector2(x, y) / (size - 1) - Utils.half_vector;
            var center = offset + Utils.half_vector;
            var pos = center + coordinates * scale;
            var height = PerlinNoiseExperiment.PerlinNoise3D(pos.x, pos.y, time, octaves);
            height *= height_scale;
            return new Vector3(coordinates.x, height, coordinates.y);
        }

        private void OnValidate()
        {
            PerlinNoiseExperiment.Instance.OnValidate();
        }
    }
}
