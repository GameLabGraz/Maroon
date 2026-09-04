using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DemoCameraScreen : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private bool isScreenOn = false;

    public void SetRenderTexture(RenderTexture value)
    {
        meshRenderer.material.mainTexture = value;
    }

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        bool isQuad = meshFilter.sharedMesh.vertexCount == 4 && meshFilter.sharedMesh.triangles.Length == 6;
        if (!isQuad)
        {
            Debug.LogError("DemoCameraScreen::Awake: need Quad mesh in Object.");
            return;
        }

        meshRenderer.enabled = isScreenOn;
    }
    public void SwitchScreenOnOff(bool value)
    {
        isScreenOn = value;
        meshRenderer.enabled = isScreenOn;
    }
    private void OnDestroy()
    {
        meshRenderer.enabled = false;
    }
}