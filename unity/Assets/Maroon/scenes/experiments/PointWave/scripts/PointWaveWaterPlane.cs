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

        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_verticesPerLength"), verticesPerLength);
        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_verticesPerWidth"), verticesPerWidth);
        // object p = _meshRenderer.additionalVertexStreams();
        n2mesh = GetComponent<MeshFilter>().mesh.vertices;
        n1mesh = GetComponent<MeshFilter>().mesh.vertices;

        //  int sizeOfMesh = GetComponent<MeshFilter>().mesh.vertices.Length;
        int sizeOfMesh = 4 * 33000;
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(float));
        m_ShadowParamsCB = new ComputeBuffer(1, sizeOfMesh);
        Graphics.SetRandomWriteTarget(2, m_ShadowParamsCB, true);
        _meshRenderer.sharedMaterial.SetBuffer(Shader.PropertyToID("_uy"), m_ShadowParamsCB);
    }


    // Tim testing

    public void RemoveClick()
    {
        //  _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_ClickedOn"), 0); I guess it works 
    }
    public void AddMouseData(Vector3 data, int hit)
    {
        Vector3[] mesh = GetComponent<MeshFilter>().mesh.vertices;

        var test = _meshRenderer.bounds.size;
        Debug.Log("Mesh  size  = " + test);
        //Debug.Log(data);
        data.x = Mathf.FloorToInt(data.x * (verticesPerLength));
        data.z = Mathf.FloorToInt(data.z * verticesPerWidth);
        Vector3 index = vertextIndex((int)data.x, (int)data.z);
        Vector3 indexp1 = vertextIndex((int)data.x + 1, (int)data.z);
        Vector3 indexm1 = vertextIndex((int)data.x - 1, (int)data.z);
        Vector3 indezp1 = vertextIndex((int)data.x, (int)data.z + 1);
        Vector3 indezm1 = vertextIndex((int)data.x, (int)data.z - 1);

        Debug.Log("click data :" + data);
        Debug.Log("index data :" + index);
        Debug.Log("indexp1 data :" + indexp1);
        Debug.Log("indexm1 data :" + indexm1);
        Debug.Log("indezp1 data :" + indezp1);
        Debug.Log(" indezm1 data :" + indezm1);
        Debug.Log("###########");
        // set here size to 1 ;
        _meshRenderer.sharedMaterial.SetVector(Shader.PropertyToID("_ClickCoordinates"), new Vector4(data.x, data.y, data.z, 0));
        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_ClickedOn"), hit);

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

        _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_EntryCount"), entryCount);
        _meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_sourceParameters"), parametersArray);
        _meshRenderer.sharedMaterial.SetVectorArray(Shader.PropertyToID("_sourceCoordinates"), coordinatesArray);
    }


    public void RegisterWaveSource(PointWaveSource waveSource)
    {
        waveSources.Add(waveSource);
    }
    public void UnregisterWaveSource(PointWaveSource waveSource)
    {
        waveSources.Remove(waveSource);
    }

    private Vector3 vertextIndex(int x, int z)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices; // optimisation problem ? 
        int quotient = verticesPerLength + x + 1; ;
        int retval = 0;
        if (z < 1)
        {
            // Debug.LogWarning((quotient * (verticesPerWidth * 2 + 1)) - (verticesPerWidth - z) - 1);
            //  return (quotient * (verticesPerWidth * 2  + 1) ) - (verticesPerWidth -  z) - 1 ;
            retval = (quotient * (verticesPerWidth * 2 + 1)) - (verticesPerWidth - z) - 1;
        }
        else
        {
            //  Debug.LogWarning(((quotient - 1) * (verticesPerWidth * 2 + 1)) + (verticesPerWidth + z));
            retval = ((quotient - 1) * (verticesPerWidth * 2 + 1)) + (verticesPerWidth + z);
        }
        if (retval < 0 || retval > vertices.Length) // maybe ? also over? 
        {
            return new Vector3(x, 0, z); // maybe  0 0 0 ? 
        }
        return vertices[retval];

    }
  

     public IEnumerator UpdateMeshData()
    {
        int N = 60; // same as the webpage
        float DELTA_X = verticesPerWidth / N;
        float DELTA_X2 = DELTA_X * DELTA_X;
        float C = 0.04f; // speed? 
        float C2 = C * C;
        float DAMPING = 0.001f;
        float DELTA_Z = verticesPerLength / N;
        float DELTA_Z2 = DELTA_Z * DELTA_Z;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
      //  Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        var dt = Time.deltaTime;
      //  Debug.Log(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            int x = (int)vertices[i].x;
            int z = (int)vertices[i].z;
            var iPrevX = vertextIndex(x - 1, z);
            var iNextX = vertextIndex(x + 1, z);
            var iPrevZ = vertextIndex(x, z - 1);
            var iNextZ = vertextIndex(x, z + 1);
            if (vertices[i].y > 0)
            {
                Debug.LogError(vertices[i]);
            }
            /*Debug.Log("NEW VERTICES CALCULATED");
            Debug.Log(x);
            Debug.Log(z);
            Debug.Log(iPrevX);
            Debug.Log(iNextX);
            Debug.Log(iPrevZ);
            Debug.Log(iNextZ);
            // vertices[i].y += 0.001f;

            var d2x = (iNextX.y - 2 * vertices[i].y + iPrevX.y); /// DELTA_X2;
            var d2z = (iNextZ.y - 2 * vertices[i].y + iPrevZ.y); //   / DELTA_Z2;
            tmpData[i].x = C2 * (d2x + d2z);

            Debug.Log("DZF");
            Debug.Log(d2x);
            Debug.Log(d2z);
            */

            tmpData[i].x += -DAMPING * tmpData[i].x;

            tmpData[i].y += dt * tmpData[i].x;

            vertices[i].y = vertices[i].y + dt * tmpData[i].y;

            /*  if ( vertices[i].y != 0)
              {
                  Debug.LogWarning(vertices[i]);
              }*/
            //    Debug.LogWarning(vertices[i]);

            //     # use Euler integration to find the new velocity w.r.t. time
            //# and the new vertical position
            //# see https://en.wikipedia.org/wiki/Euler_integration
            //           v[i].uy += dt * v[i].ay
            //         v[i].newY = v[i].y + dt *
            //         v[i].uy
          //  Debug.LogWarning(i);
            // here is the problem, that only the start of the for loop is processed 
        }
        mesh.vertices = vertices;
        yield return null;

        /*DELTA_X = W / N
        DELTA_X2 = DELTA_X * DELTA_X
        DELTA_Z = H / N
        DELTA_Z2 = DELTA_Z * DELTA_Z*/


        // mesh.vertices = vertices;

        /* prevDeltaTime = currentDeltaTime;
         currentDeltaTime = Time.deltaTime;
         int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3));
         bufferPrev = new ComputeBuffer(n1mesh.Length, stride, ComputeBufferType.Default); // maybe not needed here biutthe stride msut be the correct size
         bufferPrevPrev = new ComputeBuffer(n1mesh.Length, stride, ComputeBufferType.Default);
         n2mesh = n1mesh;
             n1mesh = GetComponent<MeshFilter>().mesh.vertices;

             Debug.Log(n1mesh.Length);
             bufferPrev.SetData(n1mesh);
             _meshRenderer.sharedMaterial.SetBuffer("pixels", bufferPrev);
             _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DeltaTime"), currentDeltaTime);// does it work ? 
             _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_pixelsSize"), n1mesh.Length);
             bufferPrev.Release(); // Not sure if correct

         bufferPrevPrev.SetData(n2mesh);
         _meshRenderer.sharedMaterial.SetBuffer("pixelsPrev", bufferPrevPrev);
         _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DeltaTimePrev"), Time.deltaTime);// does it work ? 
         _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_pixelsSize"), n2mesh.Length);
         bufferPrevPrev.Release(); // Not sure if correct
         // add support for previous delta time */

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

    public void SetPWParameterChangeTrue()
    {
        _parameterHasChanged = true;
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

