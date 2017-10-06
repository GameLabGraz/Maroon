using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaveGenerator : MonoBehaviour
{
    [SerializeField]
    private Mesh planeMesh;

    [SerializeField]
    private GameObject planeObject;

    [SerializeField]
    private int VerticesPerLength = 40;

    [SerializeField]
    private int VerticesPerWidth = 20;

    [SerializeField]
    private float waveAmplitude;

    [SerializeField]
    private float waveLength;

    [SerializeField]
    private float waveFrequency;

    [SerializeField]
    private Vector3 propagationAxis = Vector3.right;

    private Vector3[] waveVertices;

    private float time = 0;

	private void Start ()
    {
        planeMesh = new Mesh();

        // create vertices
        List<Vector3> vertices = new List<Vector3>();
        for(int i = -VerticesPerLength; i <= VerticesPerLength; i++)
        {
            for (int j = -VerticesPerWidth; j <= VerticesPerWidth; j++)
            {
                vertices.Add(new Vector3(i, 0, j));
            }
        }
        planeMesh.vertices = vertices.ToArray();

        // create triangles
        List<int> triangles = new List<int>();
        for(int i = 0; i < vertices.Count - (VerticesPerWidth * 2 + 1) - 1; i++)
        {
            if ((i + 1) % (VerticesPerWidth * 2 + 1) == 0)
                continue;

            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + (VerticesPerWidth * 2) + 2);

            triangles.Add(i + (VerticesPerWidth * 2) + 2);         
            triangles.Add(i + (VerticesPerWidth * 2) + 1);
            triangles.Add(i);

        }

        planeMesh.triangles = triangles.ToArray();

        Vector2[] uvs = new Vector2[vertices.Count];
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        planeMesh.uv = uvs;

        MeshFilter meshFilter = planeObject.GetComponent<MeshFilter>();
        meshFilter.mesh = planeMesh;

        waveVertices = planeMesh.vertices;
    }
	
	private void FixedUpdate()
    {
        for (int i = 0; i < waveVertices.Length; i++)
        {
            Vector3 waveVertex = waveVertices[i];


            Vector3 vertex = waveVertex;
            //vertex.z = 0;

            float x = Vector3.Distance(Vector3.zero, vertex);
            waveVertex.y = waveAmplitude * Mathf.Sin(2 * Mathf.PI * waveFrequency * (time - x / (waveLength * waveFrequency)));


            waveVertices[i] = waveVertex;
        }

        time += Time.fixedDeltaTime;

        planeMesh.vertices = waveVertices;
        planeMesh.RecalculateBounds();
    }
}
