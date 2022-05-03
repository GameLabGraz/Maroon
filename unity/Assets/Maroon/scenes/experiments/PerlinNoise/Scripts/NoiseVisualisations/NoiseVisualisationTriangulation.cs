using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations
{
    public class NoiseVisualisationTriangulation : NoiseVisualisation
    {
        [SerializeField, Range(0, 5)] float height_scale = 1;
        [SerializeField, Range(0, 0.2f)] float thickness = 1;


        private List<Vector3> vertices;

        private Vector2 offset;

        private float total_noise_height = 0;
        private int noise_samples;

        private int size;


        public override void GenerateMesh(Mesh mesh)
        {
            size = NoiseExperiment.Instance.size;
            offset = new Vector2(Random.value, Random.value) * 1e2f;
            noise_samples = 0;
            total_noise_height = 0;

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
                var n = GetVertexNoise(x, 0);
                vertices.Add(n + Vector3.down * thickness);
                vertices.Add(n);
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
                var n = GetVertexNoise(size - 1, y);
                vertices.Add(n + Vector3.down * thickness);
                vertices.Add(n);
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
                var n = GetVertexNoise(x, size - 1);
                vertices.Add(n + Vector3.down * thickness);
                vertices.Add(n);
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
                var n = GetVertexNoise(0, y);
                vertices.Add(n + Vector3.down * thickness);
                vertices.Add(n);
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

            mesh.Clear();

            mesh.vertices = vertices.Select(v => v - new Vector3(0, total_noise_height / noise_samples, 0)).ToArray();
            mesh.uv = UVs.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            noise_samples = 0;
            total_noise_height = 0;
            size = NoiseExperiment.Instance.size;


            var vertex_count = size * size * 2 + size * 8;
            if (vertices.Count != vertex_count)
            {
                GenerateMesh(mesh);
                return;
            }

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
                var n = GetVertexNoise(x, 0);
                vertices[index + 2 * x] = n + Vector3.down * thickness;
                vertices[index + 2 * x + 1] = n;
            }

            index += size * 2;
            for (int y = 0; y < size; y++) //bottom skirt
            {
                var n = GetVertexNoise(size - 1, y);
                vertices[index + 2 * y] = n + Vector3.down * thickness;
                vertices[index + 2 * y + 1] = n;
            }

            index += size * 2;
            for (int x = 0; x < size; x++) //right skirt
            {
                var n = GetVertexNoise(x, size - 1);
                vertices[index + 2 * x] = n + Vector3.down * thickness;
                vertices[index + 2 * x + 1] = n;
            }

            index += size * 2;
            for (int y = 0; y < size; y++) //top skirt
            {
                var n = GetVertexNoise(0, y);
                vertices[index + 2 * y] = n + Vector3.down * thickness;
                vertices[index + 2 * y + 1] = n;
            }

            if (!mesh)
                return;

            mesh.vertices = vertices.Select(v => v - new Vector3(0, total_noise_height / noise_samples, 0)).ToArray();

            mesh.colors = vertices
                .Select(v => NoiseExperiment.Instance.GetVertexColor(v.y, -height_scale, 0, height_scale))
                .ToArray();
            mesh.RecalculateNormals();
        }


        private Vector3 GetVertexNoise(float x, float y)
        {
            var coordinates01 = new Vector2(x, y) / (size - 1) - Utils.half_vector_2;
            var center = offset + Utils.half_vector_2;
            var pos = center + coordinates01 * NoiseExperiment.Instance.scale;
            var height = NoiseExperiment.Noise3D.GetNoise2D(pos.x, pos.y, NoiseExperiment.Instance.octaves);
            height *= height_scale;

            total_noise_height += height;
            noise_samples++;

            return new Vector3(coordinates01.x, height, coordinates01.y);
        }
    }
}
