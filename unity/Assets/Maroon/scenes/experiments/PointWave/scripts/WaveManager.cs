using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public ComputeShader waveCompute;

    // Tim added
    public Material waveMaterial;
    public RenderTexture Nstate;
    public RenderTexture Prevstate;
    public RenderTexture Nextstate;
    public Vector2Int resolution;
    public Plane plane;
    private Camera cam;
    // Start is called before the first frame update
    public Mesh planeMesh;
    private Vector3 clickedData;
    public float phase = 0.99f;
    void Start()
    {
        clickedData = new Vector3(0, 0, 0);
        resolution = new Vector2Int(256, 256); // ask -- maybe performance
        initializeTexture(ref Nstate);
        initializeTexture(ref Prevstate);
        initializeTexture(ref Nextstate);
        waveCompute.SetTexture(0, "Nstate", Nstate);
        waveCompute.SetTexture(0, "Prevstate", Prevstate);
        waveCompute.SetTexture(0, "Nextstate", Nextstate);
        waveCompute.SetFloat("Resolution", Nstate.width);
        waveCompute.SetInts("dimensions", new int[] { resolution.x, resolution.y });
        waveMaterial.mainTexture = Nstate;
        cam = Camera.main;
        planeMesh = GetComponent<MeshFilter>().mesh;
        waveCompute.SetVector("Clicked", clickedData);

    }


    void initializeTexture(ref RenderTexture material)
    {
        material = new RenderTexture(resolution.x, resolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm);// yt same number;
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
                    var data = transform.InverseTransformPoint(hit.point);
                    BoxCollider col = GameObject.Find("innerBathub").GetComponent<BoxCollider>();

                    int xPixel = 0;
                    int zPixel = 0;
                    float xRes = resolution.x / 2;
                    float zRes = resolution.y / 2;

                    if (data.x > 0)
                    {
                        float prozentx = (5 - data.x) / 5;
                        xPixel = (int)((xRes * prozentx));
                    }
                    else
                    {
                        float prozentx = (5 + data.x) / 5;
                        xPixel = (int)(resolution.x - (xRes * prozentx));
                    }
                    if (data.z > 0)
                    {
                        float prozentz = (5 - data.z) / 5;
                        zPixel = (int)((zRes * prozentz));
                    }
                    else
                    {
                        float prozentz = (5 + data.z) / 5;
                        zPixel = (int)(resolution.y - (zRes * prozentz));
                    }
                    clickedData = new Vector3(xPixel, zPixel, 1); // add the height from the gui ? 
                    waveCompute.SetVector("Clicked", clickedData);
                    // plane.
                }
            }
            // Debug.Log("Pressed primary button.");
        }
        Graphics.CopyTexture(Nstate, Prevstate);
        Graphics.CopyTexture(Nextstate, Nstate);
        waveCompute.SetTexture(0, "Nstate", Nstate);
        waveCompute.SetTexture(0, "Prevstate", Prevstate);
        waveCompute.SetTexture(0, "Nextstate", Nextstate);
        waveCompute.SetFloat("phase", phase);
        waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 10);
        clickedData = new Vector3(0, 0, 0);
        waveCompute.SetVector("Clicked", clickedData);
        //  waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 1);
        //  waveMaterial.SetTexture("_MainTex", Nstate);
    }
}
