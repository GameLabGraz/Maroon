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
    // Start is called before the first frame update
    void Start()
    {
        resolution = new Vector2Int(256, 256);
        initializeTexture(ref Nstate);
        waveCompute.SetTexture(0, "Nstate", Nstate);
        waveCompute.SetFloat("Resolution", Nstate.width);
        waveMaterial.mainTexture = Nstate;

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
        waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 1);

        //  waveCompute.Dispatch(0, Nstate.width / 8, Nstate.height / 8, 1);
        //  waveMaterial.SetTexture("_MainTex", Nstate);
    }
}
