using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthFlashlight : MonoBehaviour
{
    public Camera cam;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
            //cam = this.GetComponent<Camera>();
            cam.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        if (mat == null)
        {
            mat = new Material(Shader.Find("Hidden/DepthShader"));
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
