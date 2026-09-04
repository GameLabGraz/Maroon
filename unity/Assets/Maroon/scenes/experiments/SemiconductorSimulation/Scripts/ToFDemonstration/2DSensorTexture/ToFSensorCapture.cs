// Renamed from DepthCameraTextureHandler.
// "Sensor" instead of "Camera" because this rig represents the iToF emitter/sensor
// doing the actual distance measurement - the Camera component it sits on is an
// implementation detail (Unity needs a Camera to get a depth buffer), not the concept.
using UnityEngine;
using UnityEngine.Serialization;
using Maroon.Physics;

[RequireComponent(typeof(Camera))]
public class ToFSensorCapture : MonoBehaviour
{
    [Header("Shaders")]
    [FormerlySerializedAs("heatmapDepthTexture")]
    [SerializeField] private RenderTexture scanDepthTexture;
    [FormerlySerializedAs("heatmapColourTexture")]
    [SerializeField] private RenderTexture scanHeatmapTexture;
    [FormerlySerializedAs("absorptionTexture")]
    [SerializeField] private RenderTexture scanAbsorptionTexture;
    [FormerlySerializedAs("heatmapMaterial")]

    [Header("Materials")]
    [SerializeField] private Material scanHeatmapMaterial;
    [FormerlySerializedAs("heatmapDepthMaterial")]
    [SerializeField] private Material scanDepthMaterial;
    [FormerlySerializedAs("AbsorptionMaterial")]
    [SerializeField] private Material scanAbsorptionMaterial;


    private QuantityFloat maxHeatmapDistanceQuantity = new QuantityFloat(4f) { minValue = 0.1f, maxValue = 20f };
    private QuantityFloat maxIRLightingRangeQuantity = new QuantityFloat(6.0f) { minValue = 0.1f, maxValue = 30f };

    private Camera _sensorViewCamera;

    private bool _isValid = false;


    private void Awake()
    {
        _isValid = Validate();
        _sensorViewCamera = GetComponent<Camera>();

        //To generate depth info. Needed in shader
        _sensorViewCamera.depthTextureMode |= DepthTextureMode.Depth;
        _sensorViewCamera.renderingPath = RenderingPath.DeferredShading;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) => CaptureFrame(src, dest);

    /// <summary>
    /// Postprocess where the camera applies 2 different shader to 2 render textures.
    /// Unchanged logic - only pulled out of OnRenderImage into a named, callable step.
    /// </summary>
    public void CaptureFrame(RenderTexture src, RenderTexture dest)
    {
        if (!_isValid)
        {
            Graphics.Blit(src, dest);
            return;
        }
        ToFSensorShaderFeed.UpdateScanPerspectiveShaderMatrices(_sensorViewCamera, scanHeatmapMaterial);
        Graphics.Blit(src, dest, scanHeatmapMaterial);
        Graphics.Blit(dest, scanHeatmapTexture);

        Graphics.Blit(src, scanDepthTexture, scanDepthMaterial);

        ToFSensorShaderFeed.UpdateScanPerspectiveShaderMatrices(_sensorViewCamera, scanAbsorptionMaterial);
        Graphics.Blit(src, scanAbsorptionTexture, scanAbsorptionMaterial);
    }

    /// <summary>
    /// Setting the Rendertexture resolution changes also the resolution/pixelCount of the sensor chip.
    /// </summary>
    public void SetRenderTextureResulution(RenderTexture renderTexture, int xResolution, int yResolution)
    {
        if (renderTexture.width == xResolution && renderTexture.height == yResolution)
        {
            return;
        }

        renderTexture.Release();
        renderTexture.width = xResolution;
        renderTexture.height = yResolution;
    }


   
    /// <summary>
    /// Validates requirements
    /// </summary>
    private bool Validate()
    {
        bool valid = true;

        if (scanDepthTexture == null)
        {
            Debug.LogError("ToF Sensor Capture: scan depth texture not assigned.", this);
            valid = false;
        }
        if (scanHeatmapTexture == null)
        {
            Debug.LogError("ToF Sensor Capture: scan colour texture not assigned.", this);
            valid = false;
        }
        if (scanAbsorptionTexture == null)
        {
            Debug.LogError("ToF Sensor Capture: scan absorption texture not assigned.", this);
            valid = false;
        }
        if (scanHeatmapMaterial == null)
        {
            Debug.LogError("ToF Sensor Capture: scan colour material not assigned.", this);
            valid = false;
        }
        if (scanDepthMaterial == null)
        {
            Debug.LogError("ToF Sensor Capture: scan depth material not assigned.", this);
            valid = false;
        }
        if (scanAbsorptionMaterial == null)
        {
            Debug.LogError("ToF Sensor Capture: scan absorption material not assigned.", this);
            valid = false;
        }

        return valid;
    }

    // Lazy initialization Getter since there were issues with execution order of Inits
    public Camera GetSensorCamera => 
        _sensorViewCamera != null ? _sensorViewCamera : (_sensorViewCamera = GetComponent<Camera>());

    public QuantityFloat MaxHeatmapDistanceQuantity
    {
        get => maxHeatmapDistanceQuantity;
    }

    public QuantityFloat MaxIRLightingRangeQuantity
    {
        get => maxIRLightingRangeQuantity;
    }

    public float MaxHeatmapDistance
    {
        get => maxHeatmapDistanceQuantity != null ? maxHeatmapDistanceQuantity.Value : 0f;
        set
        {
            if (maxHeatmapDistanceQuantity != null)
            {
                maxHeatmapDistanceQuantity.Value = value;
            }
        }
    }
    public float MaxIRLightingRange
    {
        get => maxIRLightingRangeQuantity != null ? maxIRLightingRangeQuantity.Value : 0f;
        set
        {
            if (maxIRLightingRangeQuantity != null)
            {
                maxIRLightingRangeQuantity.Value = value;
            }
        }
    }
    public RenderTexture HeatmapTexture => scanHeatmapTexture;
    public RenderTexture HeatmapDepthTexture => scanDepthTexture;
    public RenderTexture AbsorptionTexture => scanAbsorptionTexture;
    public Material HeatmapMaterial => scanHeatmapMaterial;
    public Material AbsorptionMaterial => scanAbsorptionMaterial;
}
