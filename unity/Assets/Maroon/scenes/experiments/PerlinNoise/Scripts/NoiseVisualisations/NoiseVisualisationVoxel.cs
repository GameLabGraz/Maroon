using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts.NoiseVisualisations
{
    public class NoiseVisualisationVoxel : NoiseVisualisation
    {
        [SerializeField] Vector3 transform_offset;
        [SerializeField] private int max_size = 30;
        public override int GetMaxSize() => max_size;

        private Noise3D noise_3d;
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<int> indices = new List<int>();
        private readonly List<Color> colors = new List<Color>();

        private int size;

        public override void GenerateMesh(Mesh mesh)
        {
            vertices.Clear();
            indices.Clear();
            colors.Clear();

            noise_3d.GenerateNoiseMap();

            size = NoiseExperimentBase.Instance.size;

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
            var dirty = noise_3d.GenerateNoiseMap();

            if (!dirty)
                return;

            GenerateMesh(mesh);
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
            noise_3d.GetNoiseMapArrayB(voxel) && !noise_3d.GetNoiseMapArrayB(voxel + direction);


        private void AddFace(IReadOnlyCollection<Vector3Int> new_vertices)
        {
            vertices.AddRange(new_vertices.Select(v => v - one * 0.5f * size + transform_offset));
            colors.AddRange(
                new_vertices.Select(v => NoiseExperimentBase.Instance.GetVertexColor(v.y, 0, size * 0.5f, size)));
            indices.AddRange(new[]
            {
                vertices.Count - 1, vertices.Count - 3, vertices.Count - 2,
                vertices.Count - 4, vertices.Count - 2, vertices.Count - 3
            });
        }


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
            noise_3d = GetComponent<Noise3D>();
        }
    }
}
