using System;
using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class PerlinNoiseExperiment : MonoBehaviour
    {
        [SerializeField, Range(3, 100)] int size = 10;
        [SerializeField, Range(0, 5)] float height_scale = 1;
        [SerializeField, Range(0, 0.2f)] float thickness = 1;
        [SerializeField, Range(0, 10)] float scale = 1;
        [SerializeField, Range(1, 10)] float octaves = 2;

        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        [SerializeField, Range(0, 20)] private float rotation_speed = 0;
        private MeshFilter meshFilter;
        private Mesh mesh;
        private List<Vector3> vertices;
        private Vector2 offset;
        private float time;
        private float rotation;
        private bool dirty;

        public void SetOctaves(float octave)
        {
            dirty = true;
            octaves = octave;
        }

        public void SetScale(float s)
        {
            dirty = true;
            scale = s;
        }

        // Start is called before the first frame update
        void Start()
        {
            GenerateMesh();
            SimulationController.Instance.StartSimulation();
        }

        // Update is called once per frame
        void Update()
        {
            if (SimulationController.Instance && SimulationController.Instance.SimulationRunning)
            {
                rotation += Time.deltaTime * rotation_speed;
                meshFilter.transform.localRotation = Quaternion.Euler(Vector3.up * rotation);
                time += Time.deltaTime * speed;
                dirty = true;
            }

            if (dirty)
                UpdateMesh();
            dirty = false;
        }

        private void UpdateMesh()
        {
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


        private void GenerateMesh()
        {
            mesh = meshFilter.sharedMesh;
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

        private Vector3 GetVertexNoise(float x, float y)
        {
            var coordinates = new Vector2(x, y) / (size - 1) - Utils.half_vector;
            var center = offset + Utils.half_vector;
            var pos = center + coordinates * scale;
            var height = PerlinNoise3D(pos.x, pos.y, time);
            height *= height_scale;
            return new Vector3(coordinates.x, height, coordinates.y);
        }

        public float PerlinNoise3D(float x, float y, float z)
        {
            var xy = PerlinNoise2D(x, y);
            var xz = PerlinNoise2D(x, z);
            var yz = PerlinNoise2D(y, z);
            var yx = PerlinNoise2D(y, x);
            var zx = PerlinNoise2D(z, x);
            var zy = PerlinNoise2D(z, y);

            return (xy + xz + yz + yx + zx + zy) / 6;
        }

        public float PerlinNoise2D(float x, float y)
        {
            var noise = 0f;
            int i;
            for (i = 1; i < octaves; i++)
                noise += (PerlinNoiseIrregular(x * i, y * i) - 0.5f) / i;
            var last_octave_fraction = octaves - i + 1;
            noise += (PerlinNoiseIrregular(x * i, y * i) - 0.5f) * last_octave_fraction / i;

            return noise;
        }

        private float PerlinNoiseIrregular(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
            return Mathf.Sin(Mathf.PI * (1 + Mathf.PerlinNoise(x, y)));
        }

        private void OnValidate()
        {
            if (!meshFilter)
                meshFilter = GetComponentInChildren<MeshFilter>();
            if (!meshFilter)
                return;

            mesh = meshFilter.sharedMesh;
            var vertex_count = size * size * 2 + size * 8;
            if (!mesh || mesh.vertexCount == vertex_count)
            {
                try
                {
                    UpdateMesh();
                }
                catch (Exception)
                {
                    GenerateMesh();
                }

                return;
            }

            GenerateMesh();
            return;
            if (!EditorApplication.update.GetInvocationList().Contains((Action) Update))
                EditorApplication.update += Update;
        }
    }

    public static class Utils
    {
        public static Vector2 half_vector => new Vector2(0.5f, 0.5f);
    }
}
