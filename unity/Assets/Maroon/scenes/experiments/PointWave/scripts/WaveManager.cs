using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public ComputeShader waveCompute;

    // Tim added
    public Material waveMaterial;
    public RenderTexture Nstate;
    public RenderTexture Nm1state;
    public RenderTexture Np1state;
    public Vector2Int resolution;
    public Plane plane;
    private Camera cam;
    // Start is called before the first frame update
    public Mesh planeMesh;
    void Start()
    {
        resolution = new Vector2Int(256,256); // ask -- maybe performance
        initializeTexture(ref Nstate);
        initializeTexture(ref Nm1state);
        initializeTexture(ref Np1state);
        waveCompute.SetTexture(0, "Nstate", Nstate);
        waveCompute.SetFloat("Resolution", Nstate.width);
        waveMaterial.mainTexture = Nstate;
        cam = Camera.main;
        planeMesh = GetComponent<MeshFilter>().mesh;

    }


    void initializeTexture(ref RenderTexture material)
    {
        material = new RenderTexture(resolution.x, resolution.y, 28);// yt same number;
        material.enableRandomWrite = true;
        material.Create();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f)) // refactor
            {
     
                if (hit.transform.gameObject == GameObject.Find("innerBathub")) // can add boundary maybe
                {
                    Vector3 worldPosition = new Vector3();
                  //   GetComponent<BoxCollider>();
                    var data = transform.InverseTransformPoint(hit.point);
                    BoxCollider col =  GameObject.Find("innerBathub").GetComponent<BoxCollider>();
                    Vector4 clickedData;
                    worldPosition = data;
                    worldPosition.x = (data.x / (col.size.x / 2)); ;
                    worldPosition.z = (data.z / (col.size.z / 2)); ;
                    int xPixel = 100;
                    int zPixel = 100; ;
                    Debug.Log("data");
                    Debug.Log(data.x);
                    Debug.Log(data.z);

                    if (data.x > 0 )
                    {
                        float xRes = resolution.x / 2;
                        float prozentx = (5 - data.x) / 5;
                        xPixel = (int)((xRes * prozentx));
                    }
                    else
                    {
                        float xRes = resolution.x / 2;
                        float prozentx = (5 + data.x) / 5;

                        xPixel = (int)(resolution.x - (xRes * prozentx));
                    }
                    if (data.z > 0)
                    {
                        float zRes = resolution.y / 2;
                        float prozentx = (5 - data.z) / 5;
                        zPixel = (int)((zRes * prozentx));
                    }
                    else
                    {
                        float zRes = resolution.y / 2;
                        float prozentx = (5 + data.z) / 5;
                        zPixel = (int)(resolution.y - (zRes * prozentx));
                    }
                    clickedData = new Vector4(xPixel, 0, zPixel,0); // add the height from the gui ? 
                    Debug.Log("lcikced pixel");
                    Debug.Log(clickedData);
                    waveCompute.SetVector("Clicked", clickedData);
                    // plane.
                }
            }
           // Debug.Log("Pressed primary button.");
        }
        waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 1);

        //  waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 1);
        //  waveMaterial.SetTexture("_MainTex", Nstate);
    }
}
