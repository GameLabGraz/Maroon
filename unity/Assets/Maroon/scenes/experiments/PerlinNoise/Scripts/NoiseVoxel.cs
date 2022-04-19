using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class NoiseVoxel : NoiseExperiment
    {
        [SerializeField] private Vector2 threshold_3d_range;
        [SerializeField] private Vector2 threshold_2d_range;
        [SerializeField] private float time_scale = 1;

        [SerializeField] private QuantityFloat scale;
        [SerializeField] private QuantityFloat octaves;
        [SerializeField] private QuantityFloat flatness;

        [SerializeField, Range(3, 50)] int size = 10;
        [SerializeField] Vector3 transform_offset;

        [Space(10)] [SerializeField, Range(0, 2)]
        float speed = 1;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<int> indices = new List<int>();
        private readonly List<Color> colors = new List<Color>();
        
        private Vector3 offset;

        private void Awake()
        {
            offset = new Vector3(Random.value, Random.value, Random.value) * 1e2f;
        }

        private bool[] noise_map_array;

        private void SetNoiseMapArray(int x, int y, int z, bool value) =>
            noise_map_array[x * size * size + y * size + z] = value;

        private void SetNoiseMapArray(Vector3Int index, bool value) =>
            SetNoiseMapArray(index.x, index.y, index.z, value);

        private bool GetNoiseMapArray(int x, int y, int z) =>
            IsValidNoiseArrayIndex(x, y, z) && noise_map_array[x * size * size + y * size + z];

        private bool GetNoiseMapArray(Vector3Int index) => GetNoiseMapArray(index.x, index.y, index.z);

        private bool IsValidNoiseArrayIndex(int x, int y, int z) =>
            x.IsInRange(0, size - 1) && y.IsInRange(0, size - 1) && z.IsInRange(0, size - 1);

        public override void GenerateMesh(Mesh mesh)
        {
            vertices.Clear();
            indices.Clear();
            colors.Clear();
            if (noise_map_array?.Length != size * size * size)
                noise_map_array = new bool[size * size * size];

            FillNoiseMap(out _);

            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < size; voxel.z++)
                    {
                        AddVoxelVertices(voxel);
                    }
                }
            }

            mesh.Clear();

            mesh.vertices = vertices.Select(v => v / size).ToArray();
            mesh.triangles = indices.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            if(PerlinNoiseExperiment.Instance.noise_experiment != this)
                return;

            vertices.Clear();
            indices.Clear();
            colors.Clear();

            if (noise_map_array?.Length != size * size * size)
                noise_map_array = new bool[size * size * size];

            FillNoiseMap(out var dirty);
            if(!dirty)
                return;

            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < size; voxel.z++)
                    {
                        AddVoxelVertices(voxel);
                    }
                }
            }

            mesh.Clear();

            mesh.indexFormat = vertices.Count > ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            mesh.vertices = vertices.Select(v => v / size).ToArray();
            mesh.triangles = indices.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();
        }

        private void FillNoiseMap(out bool dirty)
        {
            dirty = false;
            var voxel = new Vector3Int();

            for (voxel.x = 0; voxel.x < size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < size; voxel.z++)
                    {
                        var coordinates01 = (Vector3)voxel / (size - 1) - Utils.half_vector_3;
                        var center = offset + Utils.half_vector_3;
                        var noise_pos = center + coordinates01 * scale;
                        var time = PerlinNoiseExperiment.Instance.time * time_scale;
                        var n3 = PerlinNoiseExperiment.PerlinNoise3D(noise_pos.x, noise_pos.y, noise_pos.z);
                        var n2 = PerlinNoiseExperiment.PerlinNoise2D(noise_pos.x, noise_pos.z);
                        n3 = n3.Map(threshold_3d_range.x, threshold_3d_range.y);
                        n2 = n2.Map(threshold_2d_range.x, threshold_2d_range.y);
                        var n = n3 * (1 - flatness) + (n2 - (float)voxel.y / size) * flatness;
                        n = BorderAdjustment(voxel, n);
                        var fill = n > 0;

                        if (GetNoiseMapArray(voxel) != fill)
                            dirty = true;
                        SetNoiseMapArray(voxel, fill);
                    }
                }
            }
        }

        private float BorderAdjustment(Vector3Int voxel, float n)
        {

            var min_value = Mathf.Min(voxel.x, voxel.y, voxel.z);
            var max_value = Mathf.Max(voxel.x, voxel.y, voxel.z);
            min_value = Mathf.Min(min_value, Mathf.Abs(max_value - size));
            
            return n - Mathf.Max(1 / (min_value + 0.3f) - 0.2f, 0);
        }

        //not doing this would allocate a new vector everytime we use this
        private readonly Vector3Int front = new Vector3Int(0, 0, 1);
        private readonly Vector3Int back = new Vector3Int(0, 0, -1);
        private readonly Vector3Int left = Vector3Int.left;
        private readonly Vector3Int right = Vector3Int.right;
        private readonly Vector3Int down = Vector3Int.down;
        private readonly Vector3Int up = Vector3Int.up;
        private readonly Vector3Int left_up = Vector3Int.left + Vector3Int.up;
        private readonly Vector3Int front_up = new Vector3Int(0, 1, 1);
        private readonly Vector3Int front_left = new Vector3Int(-1, 0, 1);
        private readonly Vector3Int front_up_left = new Vector3Int(-1, 1, 1);
        private readonly Vector3 one = Vector3.one;

        private void AddVoxelVertices(Vector3Int voxel)
        {
            var is_left_visible = IsFaceVisible(voxel, left);
            var is_right_visible = IsFaceVisible(voxel, right);
            var is_down_visible = IsFaceVisible(voxel, down);
            var is_up_visible = IsFaceVisible(voxel, up);
            var is_front_visible = IsFaceVisible(voxel, front);
            var is_back_visible = IsFaceVisible(voxel, back);

            if (is_left_visible)
                AddFace(new[]
                {
                    voxel + left, voxel + left_up,
                    voxel + front_left, voxel + front_up_left
                });
            if (is_right_visible)
                AddFace(new[]
                {
                    voxel, voxel + front,
                    voxel + up, voxel + front_up
                });

            if (is_down_visible)
                AddFace(new[]
                {
                    voxel, voxel + left,
                    voxel + front, voxel + front_left
                });
            if (is_up_visible)
                AddFace(new[]
                {
                    voxel + up, voxel + front_up,
                    voxel + left_up, voxel + front_up_left
                });
            if (is_front_visible)
                AddFace(new[]
                {
                    voxel + front, voxel + front_left,
                    voxel + front_up, voxel + front_up_left
                });
            if (is_back_visible)
                AddFace(new[]
                {
                    voxel, voxel + up,
                    voxel + left, voxel + left_up
                });
            // */
        }

        private bool IsFaceVisible(Vector3Int voxel, Vector3Int direction) => 
            GetNoiseMapArray(voxel) && !GetNoiseMapArray(voxel + direction);

        private void AddFace(IReadOnlyCollection<Vector3Int> new_vertices)
        {
            vertices.AddRange(new_vertices.Select(v => v - one * 0.5f * size + transform_offset));
            colors.AddRange(new_vertices.Select(v => PerlinNoiseExperiment.Instance.GetVertexColor(v.y, 0, size * 0.5f, size)));
            indices.AddRange(new[]
            {
                vertices.Count - 1, vertices.Count - 3, vertices.Count - 2,
                vertices.Count - 4, vertices.Count - 2, vertices.Count - 3
            });
        }


        private void OnValidate()
        {
            offset = new Vector3(Random.value, Random.value, Random.value) * 1e2f;
            PerlinNoiseExperiment.Instance.OnValidate();

            scale.onValueChanged.RemoveAllListeners();
            scale.onValueChanged.AddListener(_ => PerlinNoiseExperiment.Instance.SetDirty());
            flatness.onValueChanged.RemoveAllListeners();
            flatness.onValueChanged.AddListener(_ => PerlinNoiseExperiment.Instance.SetDirty());
        }
    }
}
