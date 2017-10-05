using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject planeObject;

    [SerializeField]
    private int VerticesPerSide = 40;

    [SerializeField]
    private float waveAmplitude;

    [SerializeField]
    private float waveLength;

    [SerializeField]
    private float waveFrequency;

    [SerializeField]
    private Vector3 propagationAxis = Vector3.right;

    private Vector3[] waveVertices;

    private Mesh mesh;

    private float time = 0;

	private void Start ()
    {

        /*
        Mesh testMesh = new Mesh();


        // create vertices
        List<Vector3> vertices = new List<Vector3>();
        for(int i = -VerticesPerSide; i <= VerticesPerSide; i++)
        {
            for (int j = -VerticesPerSide; j <= VerticesPerSide; j++)
            {
                vertices.Add(new Vector3(i, 0, j));
            }
        }
        testMesh.vertices = vertices.ToArray();

        // create triangles
        List<int> triangles = new List<int>();
        for(int i = 0; i < vertices.Count - (VerticesPerSide * 2 + 1) - 1; i++)
        {
            if ((i + 1) % (VerticesPerSide * 2 + 1) == 0)
                continue;

            triangles.Add(i + (VerticesPerSide * 2) + 2);
            triangles.Add(i + 1);
            triangles.Add(i);

            triangles.Add(i);
            triangles.Add(i + (VerticesPerSide * 2) + 1);
            triangles.Add(i + (VerticesPerSide * 2) + 2);
        }

        testMesh.triangles = triangles.ToArray();

        Vector2[] uvs = new Vector2[vertices.Count];
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        testMesh.uv = uvs;
        */

        MeshFilter meshFilter = planeObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        //meshFilter.mesh = testMesh;
        

        mesh = meshFilter.mesh;
        waveVertices = mesh.vertices;
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

        mesh.vertices = waveVertices;
        mesh.RecalculateBounds();
    }
}
