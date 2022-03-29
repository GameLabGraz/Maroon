using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class NoiseVoxel : MonoBehaviour, INoiseExperiment
    {
        [SerializeField, Range(0, 10)] public float scale = 1;
        [SerializeField, Range(1, 10)] public float octaves = 2;
        [SerializeField, Range(0, 1)] public float threshold = 0.5f;
        [SerializeField, Range(0, 1)] public float threshold_2d = 0.5f;

        [SerializeField, Range(3, 20)] int size = 10;

        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<int> indices = new List<int>();


        private readonly HashSet<Vector3Int> noise_map = new HashSet<Vector3Int>();

        
        
        public void SetOctaves(float octave)
        {
            PerlinNoiseExperiment.Instance.dirty = true;
            octaves = octave;
        }

        public void SetScale(float s)
        {
            PerlinNoiseExperiment.Instance.dirty = true;
            scale = s;
        }
        public void GenerateMesh(Mesh mesh)
        {
            vertices.Clear();
            indices.Clear();
            noise_map.Clear();

            FillNoiseMap();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        AddVoxelVertices(new Vector3Int(x, y, z));
                    }
                }
            }


            mesh.vertices = vertices.Select(v => (Vector3) v / size).ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
        }

        public void UpdateMesh(Mesh mesh)
        {
            GenerateMesh(mesh);
        }

        private void FillNoiseMap()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        var n = PerlinNoiseExperiment.PerlinNoise3D(x / scale, y / scale, z / scale, octaves);
                        var n2 = PerlinNoiseExperiment.PerlinNoise2D(x / scale, z / scale, octaves);
                        if (n + 0.5f < threshold || n2 - 0.5f + threshold_2d > y / scale)
                        {
                            noise_map.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }
        }

        private void AddVoxelVertices(Vector3Int voxel)
        {
            var front = new Vector3Int(0, 0, 1);
            var back = new Vector3Int(0, 0, -1);
            var is_left_visible = IsFaceVisible(voxel, Vector3Int.left);
            var is_right_visible = IsFaceVisible(voxel, Vector3Int.right);
            var is_down_visible = IsFaceVisible(voxel, Vector3Int.down);
            var is_up_visible = IsFaceVisible(voxel, Vector3Int.up);
            var is_front_visible = IsFaceVisible(voxel, front);
            var is_back_visible = IsFaceVisible(voxel, back);

            if (is_left_visible)
                AddFace(new[]
                {
                    voxel + Vector3Int.left, voxel + Vector3Int.left + Vector3Int.up,
                    voxel + Vector3Int.left + front, voxel + Vector3Int.left + Vector3Int.up + front
                });
            if (is_right_visible)
                AddFace(new[]
                {
                    voxel, voxel + front,
                    voxel + Vector3Int.up, voxel + Vector3Int.up + front
                });

            if (is_down_visible)
                AddFace(new[]
                {
                    voxel, voxel + Vector3Int.left,
                    voxel + front, voxel + Vector3Int.left + front
                });
            if (is_up_visible)
                AddFace(new[]
                {
                    voxel + Vector3Int.up, voxel + Vector3Int.up + front,
                    voxel + Vector3Int.up + Vector3Int.left, voxel + Vector3Int.up + Vector3Int.left + front
                });
            if (is_front_visible)
                AddFace(new[]
                {
                    voxel + front, voxel + front + Vector3Int.left,
                    voxel + front + Vector3Int.up, voxel + front + Vector3Int.up + Vector3Int.left
                });
            if (is_back_visible)
                AddFace(new[]
                {
                    voxel, voxel + Vector3Int.up,
                    voxel + Vector3Int.left, voxel + Vector3Int.up + Vector3Int.left
                });
            // */
        }

        private bool IsFaceVisible(Vector3Int voxel, Vector3Int direction)
        {
            return noise_map.Contains(voxel) && !noise_map.Contains(voxel + direction);
        }

        private void AddFace(IEnumerable<Vector3Int> new_vertices)
        {
            vertices.AddRange(new_vertices.Select(v => v - Vector3.one * 0.5f * scale));
            indices.AddRange(new[]
            {
                vertices.Count - 1, vertices.Count - 3, vertices.Count - 2,
                vertices.Count - 4, vertices.Count - 2, vertices.Count - 3
            });
        }


        private void OnValidate()
        {
            PerlinNoiseExperiment.Instance.OnValidate();
        }

        private void OnDrawGizmos()
        {
            return;
            foreach (var vector3Int in noise_map)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.position + (Vector3) vector3Int / scale - Vector3.one * 0.5f,
                    Vector3.one * 0.2f);
            }
        }
    }
}
