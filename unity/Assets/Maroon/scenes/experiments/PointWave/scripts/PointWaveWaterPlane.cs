using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Rendering;
[ExecuteInEditMode]
public class PointWaveWaterPlane : PausableObject, IResetObject
{
    [SerializeField]
    private List<PointWaveSource> waveSources = new List<PointWaveSource>();

    [SerializeField]
    private int verticesPerLength = 60;

    [SerializeField]
    private int verticesPerWidth = 20;

    [SerializeField, HideInInspector]
    private Mesh planeMesh;

    private int entryCount = 0;
    private bool _parameterHasChanged = false;

    private Color startMinColor = Color.blue;
    private Color startMaxColor = Color.cyan;

    float timer;
    private MeshRenderer _meshRenderer;

    private Vector4[] coordinatesArray = new Vector4[10];
    private Vector4[] parametersArray = new Vector4[10];

    private Vector3[] n2mesh = new Vector3[388332];
    private Vector3[] n1mesh = new Vector3[388332];
    // testing
    private ComputeBuffer bufferPrev;
    private ComputeBuffer bufferPrevPrev;

    private ComputeBuffer m_ShadowParamsCB;

    //private RWBuffer<float> test_;

    private float prevDeltaTime;
    private float currentDeltaTime;
    private Vector2[] tmpData;
    protected override void Start()
    {
        base.Start();
     
        _meshRenderer = GetComponent<MeshRenderer>();
        UpdateParameterAndPosition();
        tmpData = new Vector2[GetComponent<MeshFilter>().mesh.vertices.Length];

        for(int i = 0; i < tmpData.Length; i++)
        {
            tmpData[i].x = 0;
            tmpData[i].y = 0;
        }

        CalculatePlaneMesh();
    }


    // Tim testing

    public void RemoveClick()
    {
    }
    
    public void UpdateParameterAndPosition()
    {
        Vector4 empty = Vector4.zero;
        Vector4 parameter;
        Vector4 coordinates;
        entryCount = waveSources.Count;

        for (int count = 0; count < 10; count++)
        {
            if (count < entryCount)
            {
                var source = waveSources[count];
                parameter = new Vector4(source.WaveAmplitude, source.WaveLength, source.WaveFrequency, source.WavePhase);
                coordinates = new Vector4(source.transform.position.x, source.transform.position.y, source.transform.position.z, 0);
            }
            else
            {
                parameter = empty;
                coordinates = empty;
            }

            coordinatesArray[count] = coordinates;
            parametersArray[count] = parameter;
        }
    }


    public void RegisterWaveSource(PointWaveSource waveSource)
    {
        waveSources.Add(waveSource);
    }
    public void UnregisterWaveSource(PointWaveSource waveSource)
    {
        waveSources.Remove(waveSource);
    }


      protected override void HandleUpdate()
    {
        timer += Time.deltaTime;
        _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_SceneTime"), timer);
      
        if (!_parameterHasChanged) { return; }
        
        _parameterHasChanged = false;
    }

    protected override void HandleFixedUpdate()
    {
       
    }

    public void ResetObject()
    {
        timer = 0;
        GetComponent<Renderer>().material.SetColor("_ColorMin", startMinColor);
        GetComponent<Renderer>().material.SetColor("_ColorMax", startMaxColor);
    }


    public void CalculatePlaneMesh()
    {
        planeMesh = new Mesh();
        // create vertices
        var vertices = new List<Vector3>();
        for (var i = -verticesPerLength; i <= verticesPerLength; i++)
        {
            for (var j = -verticesPerWidth; j <= verticesPerWidth; j++)
            {
                vertices.Add(new Vector3(i, 0, j));
            }
        }
        planeMesh.vertices = vertices.ToArray();

        // create triangles
        var triangles = new List<int>();
        for (var i = 0; i < vertices.Count - (verticesPerWidth * 2 + 1) - 1; i++)
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

        var uvs = new Vector2[vertices.Count];
        for (var i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        planeMesh.uv = uvs;

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = planeMesh;
    }
}  

