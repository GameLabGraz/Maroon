using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

[ExecuteInEditMode]
public class WaterPlane : PausableObject, IResetObject
{
    [SerializeField]
    private int verticesPerLength = 40;

    [SerializeField]
    private int verticesPerWidth = 20;

    [SerializeField]
    private Material material;

    [SerializeField, HideInInspector]
    private Mesh planeMesh;

    //[SerializeField]
    private List<WaveGenerator> waveGenerators = new List<WaveGenerator>();

    [SerializeField]
    private uint updateRate = 5;

    private Vector3[] waveVertices; 

    private float time = 0;

    private ulong updateCount = 0;

    private Color startMinColor;

    private Color startMaxColor;

    protected override void Start()
    {
        base.Start();

        if (planeMesh == null)
        {
            UpdatePlane();
        }

        waveVertices = planeMesh.vertices;

        startMinColor = GetComponent<Renderer>().sharedMaterial.GetColor("_ColorMin");
        startMaxColor = GetComponent<Renderer>().sharedMaterial.GetColor("_ColorMax");
    }

    private float GetTotalWaveValue(Vector3 position)
    {
        float waveValue = 0;
        foreach(WaveGenerator waveGenerator in waveGenerators)
        {
            Vector3 worldPosition = transform.TransformPoint(position);
            Vector3 worldWaveStartingPoint = waveGenerator.transform.position; //transform.TransformPoint(waveGenerator.getStaringPoint());

            Vector3 rayDirection = worldWaveStartingPoint - worldPosition;
            float maxRayLength = Vector3.Distance(worldWaveStartingPoint, worldPosition);

            if (!Physics.Raycast(worldPosition, rayDirection, maxRayLength))
                waveValue += waveGenerator.GetWaveValue(worldPosition, time);
        }           

        return waveValue;
    }

    public void UpdatePlane()
    {
        planeMesh = new Mesh();

        // create vertices
        List<Vector3> vertices = new List<Vector3>();
        for (int i = -verticesPerLength; i <= verticesPerLength; i++)
        {
            for (int j = -verticesPerWidth; j <= verticesPerWidth; j++)
            {
                vertices.Add(new Vector3(i, 0, j));
            }
        }
        planeMesh.vertices = vertices.ToArray();

        // create triangles
        List<int> triangles = new List<int>();
        for (int i = 0; i < vertices.Count - (verticesPerWidth * 2 + 1) - 1; i++)
        {
            if ((i + 1) % (verticesPerWidth * 2 + 1) == 0)
                continue;

            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + (verticesPerWidth * 2) + 2);

            triangles.Add(i + (verticesPerWidth * 2) + 2);
            triangles.Add(i + (verticesPerWidth * 2) + 1);
            triangles.Add(i);

        }

        planeMesh.triangles = triangles.ToArray();

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        planeMesh.uv = uvs;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = planeMesh;

        
    }

    public void UpdateMaterial()
    {
        GetComponent<Renderer>().material = material;
    }

    public void RegisterWaveGenerator(WaveGenerator waveGenerator)
    {
        waveGenerators.Add(waveGenerator);
    }
    public void UnregisterWaveGenerator(WaveGenerator waveGenerator)
    {
        waveGenerators.Remove(waveGenerator);
    }

    protected override void HandleUpdate()
    {
        material = GetComponent<Renderer>().sharedMaterial;
    }

    protected override void HandleFixedUpdate()
    {
        if (updateCount++ % updateRate != 0)
            return;


        for (int i = 0; i < waveVertices.Length; i++)
        {
            Vector3 waveVertex = waveVertices[i];
            waveVertex.y = GetTotalWaveValue(waveVertex);

            waveVertices[i] = waveVertex;
        }

        time += Time.fixedDeltaTime;

        planeMesh.vertices = waveVertices;
        planeMesh.RecalculateBounds();
    }

    public void ResetObject()
    {
        GetComponent<Renderer>().material.SetColor("_ColorMin", startMinColor);
        GetComponent<Renderer>().material.SetColor("_ColorMax", startMaxColor);

        for (int i = 0; i < waveVertices.Length; i++)
        {
            Vector3 waveVertex = waveVertices[i];
            waveVertex.y = 0;

            waveVertices[i] = waveVertex;
        }

        planeMesh.vertices = waveVertices;
        planeMesh.RecalculateBounds();

        time = 0;
    }
}
