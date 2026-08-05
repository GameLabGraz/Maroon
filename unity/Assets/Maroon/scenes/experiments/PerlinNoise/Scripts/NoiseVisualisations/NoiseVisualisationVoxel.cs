using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.ComputerScience.PerlinNoise.NoiseVisualisations
{
    public class NoiseVisualisationVoxel : NoiseVisualisation
    {
        [SerializeField] private Vector3 transformOffset;
        [SerializeField] private int maxSize = 30;

        private readonly List<int> _indices = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();

        private Noise3D _noise_3d;

        private int _size;
        private readonly Vector3[] _vec = new Vector3[4];
        public float Threshold => _noise_3d.threshold;

        private void Awake() => Init();

        private void OnValidate() => Init();

        public override int GetMaxSize() => maxSize;

        public override void GenerateMesh(Mesh mesh)
        {
            _noise_3d.GenerateNoiseMap();
            GenerateMeshInternal(mesh);
        }

        private void GenerateMeshInternal(Mesh mesh)
        {
            _vertices.Clear();
            _indices.Clear();

            _size = NoiseExperimentBase.Instance.size;

            var voxel = new Vector3Int();
            for (voxel.x = 0; voxel.x < _size; voxel.x++)
            {
                for (voxel.y = 0; voxel.y < _size; voxel.y++)
                {
                    for (voxel.z = 0; voxel.z < _size; voxel.z++)
                    {
                        AddVoxelVertices(voxel);
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _indices.ToArray();
            mesh.colors = _vertices.Select(v =>
                    NoiseExperimentBase.Instance.GetVertexColor(v.y + 0.5f + NoiseExperimentBase.noise.GetNoise3D(v * 0.2f)))
                .ToArray();
            mesh.RecalculateNormals();
        }

        public override void UpdateMesh(Mesh mesh)
        {
            var dirty = _noise_3d.GenerateNoiseMap();

            if (!dirty)
                return;

            GenerateMeshInternal(mesh);
        }

        private void AddVoxelVertices(Vector3Int voxel)
        {
            var front = new Vector3Int(0, 0, 1);
            var frontLeft = new Vector3Int(-1, 0, 1);
            var frontUp = new Vector3Int(0, 1, 1);
            var frontUpLeft = new Vector3Int(-1, 1, 1);
            var leftUp = new Vector3Int(-1, 1, 0);


            var isLeftVisible = IsFaceVisible(voxel, Vector3Int.left);
            var isRightVisible = IsFaceVisible(voxel, Vector3Int.right);
            var isDownVisible = IsFaceVisible(voxel, Vector3Int.down);
            var isUpVisible = IsFaceVisible(voxel, Vector3Int.up);
            var isFrontVisible = IsFaceVisible(voxel, front);
            var isBackVisible = IsFaceVisible(voxel, new Vector3Int(0, 0, -1));

            if (isLeftVisible)
            {
                _vec[0] = voxel + Vector3Int.left;
                _vec[1] = voxel + leftUp;
                _vec[2] = voxel + frontLeft;
                _vec[3] = voxel + frontUpLeft;
                AddFace(_vec);
            }

            if (isRightVisible)
            {
                _vec[0] = voxel;
                _vec[1] = voxel + front;
                _vec[2] = voxel + Vector3Int.up;
                _vec[3] = voxel + frontUp;
                AddFace(_vec);
            }

            if (isDownVisible)
            {
                _vec[0] = voxel;
                _vec[1] = voxel + Vector3Int.left;
                _vec[2] = voxel + front;
                _vec[3] = voxel + frontLeft;
                AddFace(_vec);
            }

            if (isUpVisible)
            {
                _vec[0] = voxel + Vector3Int.up;
                _vec[1] = voxel + frontUp;
                _vec[2] = voxel + leftUp;
                _vec[3] = voxel + frontUpLeft;
                AddFace(_vec);
            }

            if (isFrontVisible)
            {
                _vec[0] = voxel + front;
                _vec[1] = voxel + frontLeft;
                _vec[2] = voxel + frontUp;
                _vec[3] = voxel + frontUpLeft;
                AddFace(_vec);
            }

            if (isBackVisible)
            {
                _vec[0] = voxel;
                _vec[1] = voxel + Vector3Int.up;
                _vec[2] = voxel + Vector3Int.left;
                _vec[3] = voxel + leftUp;
                AddFace(_vec);
            }
        }

        private bool IsFaceVisible(Vector3Int voxel, Vector3Int direction) =>
            _noise_3d.GetNoiseMapArrayB(voxel) && !_noise_3d.GetNoiseMapArrayB(voxel + direction);


        private void AddFace(IList<Vector3> newVertices)
        {
            for (var i = 0; i < 4; i++)
                newVertices[i] = (newVertices[i] - Vector3.one * (0.5f * _size) + transformOffset) / _size;
            _vertices.AddRange(newVertices);
            _indices.AddRange(new[]
            {
                _vertices.Count - 1, _vertices.Count - 3, _vertices.Count - 2,
                _vertices.Count - 4, _vertices.Count - 2, _vertices.Count - 3
            });
        }

        private void Init() => _noise_3d = GetComponent<Noise3D>();
    }
}