using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.PerlinNoise.Scripts
{
    public class PerlinNoiseExperiment : MonoBehaviour
    {

        [SerializeField] int size = 10;
        [SerializeField] int scale = 1;
        [SerializeField] int height_scale = 1;
        [SerializeField] private MeshFilter meshFilter;
        private Mesh mesh;
        private List<Vector3> vertices;
        private Vector2 offset;

        // Start is called before the first frame update
        void Start()
        {
            GenerateMesh();
        }

        // Update is called once per frame
        void Update()
        {
        
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    vertices[x * size + y] = GetVertexNoise(x, y);
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.RecalculateNormals();
        }


        private void GenerateMesh()
        {
            
            mesh = meshFilter.sharedMesh;
            offset = new Vector2(Random.value, Random.value) * 1e2f;
            
            vertices = new List<Vector3>(size * size);
            var UVs = new List<Vector2>();
            var indices = new List<int>();
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    
                    vertices.Add(GetVertexNoise(x, y));
                    UVs.Add(new Vector2(x, y));
                
                    if(x == 0 || y == 0)
                        continue;
                
                    indices.AddRange(new[]
                    {
                        vertices.Count - 1, vertices.Count - 2, vertices.Count - 1 - size,
                        vertices.Count - 2 - size, vertices.Count - 1 - size, vertices.Count - 2
                    });
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.uv = UVs.ToArray();
            mesh.triangles = indices.ToArray();
            
            
        
            mesh.RecalculateNormals();
        }

        private Vector3 GetVertexNoise(int x, int y)
        {
            var pos = (offset + new Vector2(x, y)) * scale;
            var height = Mathf.PerlinNoise(pos.x, pos.y) * height_scale;
            var pos_offset = size * -0.5f;
            return new Vector3(x + pos_offset, height, y + pos_offset);
        }

        private void OnValidate()
        {
            GenerateMesh();
        }
    }
}
