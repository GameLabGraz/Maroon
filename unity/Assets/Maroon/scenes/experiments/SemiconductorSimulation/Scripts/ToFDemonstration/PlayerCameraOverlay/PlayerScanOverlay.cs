using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerScanOverlay : MonoBehaviour
{
    [SerializeField] private Material _scanVisualizerMaterial;
    [SerializeField] private Material _scanVisualizerDebugMaterial;
    private Camera _playerCamera;
    public Material ActiveMaterial { get; set; }

    /// <summary>
    /// Setup camera and turn on depth mode
    /// </summary>
    void Awake()
    {
        _playerCamera = GetComponent<Camera>();
        _playerCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    // is called when player camera renders
    void OnRenderImage(RenderTexture src, RenderTexture dest) => RenderOverlay(src, dest);

    /// <summary>
    /// Overlay the materialshader effect for the player camera 
    /// </summary>
    public void RenderOverlay(RenderTexture src, RenderTexture dest)
    {
        if (ActiveMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }
        Graphics.Blit(src, dest, ActiveMaterial);
    }

    public Material ScanVisualizerMaterial => _scanVisualizerMaterial;
    public Material ScanVisualizerDebugMaterial => _scanVisualizerDebugMaterial;
    public Camera GetCamera => _playerCamera;
}
