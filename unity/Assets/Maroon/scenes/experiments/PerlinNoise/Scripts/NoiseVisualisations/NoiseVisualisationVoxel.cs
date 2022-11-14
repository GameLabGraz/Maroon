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

        private int size;

        public override void GenerateMesh(Mesh mesh)
        {
            noise_3d.GenerateNoiseMap();
            GenerateMeshInternal(mesh);
        }

        private void GenerateMeshInternal(Mesh mesh)
        {
            vertices.Clear();
            indices.Clear();

            size = NoiseExperiment.Instance.size;

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
            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.colors = vertices.Select(v_ => NoiseExperiment.Instance.GetVertexColor(v_.y, 0, size * 0.5f, size))
                .ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            var dirty = noise_3d.GenerateNoiseMap();

            if (!dirty)
                return;

            GenerateMeshInternal(mesh);
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

        private Vector3[] vec = new Vector3[4];

        private void AddVoxelVertices(Vector3Int voxel)
        {
            var is_left_visible = IsFaceVisible(voxel, left);
            var is_right_visible = IsFaceVisible(voxel, right);
            var is_down_visible = IsFaceVisible(voxel, down);
            var is_up_visible = IsFaceVisible(voxel, up);
            var is_front_visible = IsFaceVisible(voxel, front);
            var is_back_visible = IsFaceVisible(voxel, back);

            if (is_left_visible)
            {
                vec[0] = voxel + left;
                vec[1] = voxel + left_up;
                vec[2] = voxel + front_left;
                vec[3] = voxel + front_up_left;
                AddFace(vec);
            }

            if (is_right_visible)
            {
                vec[0] = voxel;
                vec[1] = voxel + front;
                vec[2] = voxel + up;
                vec[3] = voxel + front_up;
                AddFace(vec);
            }

            if (is_down_visible)
            {
                vec[0] = voxel;
                vec[1] = voxel + left;
                vec[2] = voxel + front;
                vec[3] = voxel + front_left;
                AddFace(vec);
            }

            if (is_up_visible)
            {
                vec[0] = voxel + up;
                vec[1] = voxel + front_up;
                vec[2] = voxel + left_up;
                vec[3] = voxel + front_up_left;
                AddFace(vec);
            }

            if (is_front_visible)
            {
                vec[0] = voxel + front;
                vec[1] = voxel + front_left;
                vec[2] = voxel + front_up;
                vec[3] = voxel + front_up_left;
                AddFace(vec);
            }

            if (is_back_visible)
            {
                vec[0] = voxel;
                vec[1] = voxel + up;
                vec[2] = voxel + left;
                vec[3] = voxel + left_up;
                AddFace(vec);
            }
        }

        private bool IsFaceVisible(Vector3Int voxel, Vector3Int direction) =>
            noise_3d.GetNoiseMapArrayB(voxel) && !noise_3d.GetNoiseMapArrayB(voxel + direction);


        private void AddFace(IList<Vector3> new_vertices)
        {
            for (int i = 0; i < 4; i++)
                new_vertices[i] = (new_vertices[i] - one * (0.5f * size) + transform_offset) / size;
            vertices.AddRange(new_vertices);
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