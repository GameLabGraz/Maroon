using System;
using Maroon.Physics.HuygensPrinciple;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

[ExecuteInEditMode]
public class WaterPlane : PausableObject, IResetObject
{
    [SerializeField]
    private int verticesPerLength = 60;

    [SerializeField]
    private int verticesPerWidth = 20;
   
    [SerializeField, HideInInspector]
    private Mesh planeMesh;

    [SerializeField]
    private Material material;

    [SerializeField]
    private SlitPlate slitPlate;

    [SerializeField]
    private GameObject waterBasinGeneratorPosition;
    
    [SerializeField]
    private bool useMaterialReset = true;

    private List<WaveGenerator> _waveGenerators = new List<WaveGenerator>();

    private struct ShaderConversionStruct
    {
        public Vector4[] coordinates;
        public float[] parameters;
    };

    private float _timer;
    private MeshRenderer _meshRenderer;

    private Color _startMinColor;
    private Color _startMaxColor;

    private static int _maxNumberOfBasinGenerators = 30;
    private static int _maxNumberOfPlateGenerators = 30;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>(); 
    }

    protected override void Start()
    {
        base.Start();

        if(planeMesh == null)
        {
            CalculatePlaneMesh();
        }

        if (!_meshRenderer)
            _meshRenderer = GetComponent<MeshRenderer>();
        slitPlate = FindObjectOfType<SlitPlate>(); 
       
        if(slitPlate == null)
        {
            Debug.LogError("Slit Plate not found");
        }

        _startMinColor = _meshRenderer.sharedMaterial.GetColor("_ColorMin");
        _startMaxColor = _meshRenderer.sharedMaterial.GetColor("_ColorMax");

        Init();
        UpdatePlatePosition();
    }
    
    private void Init()
    {
        CalculatePlaneMesh();
        UpdateParameters();
    }

    public void UpdateParameters()
    {
        UpdateWaveLength();
        UpdateWaveFrequency();
        UpdatePlatePosition();
    }

    public void UpdatePlane()
    {
        var basinGenerators = WaveGeneratorPoolHandler.Instance.GetGeneratorListOfType(WaveGenerator.GeneratorMembership.WaterBasin);
        var plateGenerators = WaveGeneratorPoolHandler.Instance.GetGeneratorListOfType(WaveGenerator.GeneratorMembership.SlitPlate1);

        var basin = SetCoordinatesAndParameterArrays(basinGenerators, _maxNumberOfBasinGenerators);
        var plate = SetCoordinatesAndParameterArrays(plateGenerators, _maxNumberOfPlateGenerators);

        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_BasinEntryCount"), basinGenerators.Count);
        _meshRenderer.sharedMaterial.SetFloatArray(Shader.PropertyToID("_sourceParameters"), basin.parameters);
        _meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_sourceCoordinates"), basin.coordinates);

        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_PlateEntryCount"), plateGenerators.Count);
        _meshRenderer.sharedMaterial.SetFloatArray(Shader.PropertyToID("_plateParameters"), plate.parameters);
        _meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_plateCoordinates"), plate.coordinates);
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

    private ShaderConversionStruct SetCoordinatesAndParameterArrays(List<WaveGenerator> list, int maxArraySize)
    {
        float parameter;
        Vector4 coordinates;
        var empty = new Vector4(0f, 0f, 0f, 0f);
       
        var coordinatesArray = new Vector4[maxArraySize];
        var parametersArray = new float[maxArraySize];

        for (var count = 0; count < maxArraySize; count++)
        {
            if(count < list.Count)
            {
                coordinates = new Vector4(list[count].transform.position.x, list[count].transform.position.y, list[count].transform.position.z, 0);
                parameter = list[count].WaveAmplitude;
            }
            else
            {
                coordinates = empty;
                parameter = 0f;
            }

            coordinatesArray[count] = coordinates;
            parametersArray[count] = parameter;
        }

        ShaderConversionStruct conversion;
        conversion.coordinates = coordinatesArray;
        conversion.parameters = parametersArray; 

        return conversion;
    }

    public void UpdateMaterial()
    {
        _meshRenderer.material = material;
    }

    public void RegisterWaveGenerator(WaveGenerator waveGenerator)
    {
        _waveGenerators.Add(waveGenerator);
    }
    public void UnregisterWaveGenerator(WaveGenerator waveGenerator)
    {
        _waveGenerators.Remove(waveGenerator);
    }

    protected override void HandleUpdate()
    {
        _timer += Time.deltaTime;
        _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_SceneTime"), _timer);
    }

    protected override void HandleFixedUpdate()
    {
    }

    public void ResetObject()
    {
        if (useMaterialReset)
        {
            _meshRenderer.material.SetColor("_ColorMin", _startMinColor);
            _meshRenderer.material.SetColor("_ColorMax", _startMaxColor);
        }

        UpdateWaveLength();
        UpdateWaveFrequency();

        _timer = 0;
    }

    public void UpdatePlatePosition()
    {
        if (!_meshRenderer || !slitPlate)
            return;
        _meshRenderer.sharedMaterial.SetVector(Shader.PropertyToID("_PlatePosition"), slitPlate.transform.position);
        UpdatePlane();
    }

    public void UpdateWaveLength()
    {
        _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_WaveLength"),
            (WaveGeneratorPoolHandler.Instance.WaveLength * 0.3f));
    }

    public void UpdateWaveFrequency()
    {
        _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_WaveFrequency"),
            (WaveGeneratorPoolHandler.Instance.WaveFrequency));
    }

    public void UpdateWavePeakColor(Color col)
    {
        _meshRenderer.material.SetColor("_ColorMax", col);
    }

    public void UpdateWaveTroughColor(Color col)
    {
        _meshRenderer.material.SetColor("_ColorMin", col);
    }
}
