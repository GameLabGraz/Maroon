using UnityEngine;

/// <summary>
/// Handels the control flow for the demonstration feature of the ToF camera. 
/// Lateupdate forces sensor camera to render and that triggers onPreRender and therefore HandleCameraPreRender
/// </summary>
public class ToFDemonstrationController : MonoBehaviour, IResetObject
{
    [Header("Scripts for which control is needed")]
    [SerializeField] private ToFSensorCapture sensorCapture;
    [SerializeField] private PlayerCameraResolver cameraResolver;
    [SerializeField] private DemoCameraScreen demoCameraScreen;
    [SerializeField] private ScanOverlaySettings overlaySettings;

    // TODO decide if noise of game should be used for demo
    private RenderTexture noiseRenderTexture { get; set; }

    private UnityEngine.Events.UnityAction<float> _onMaxColorDistanceChanged;
    private UnityEngine.Events.UnityAction<float> _onMaxIRLightingRangeChanged;
    private bool _isValid;
    private bool _demoActive = true;
    private float _storedColourIntensity;
    [SerializeField] private CaptureMode captureMode = CaptureMode.DepthInformation;
    
    enum CaptureMode 
    {
        DepthInformation,
        AbsorptionValues
    }

    private bool _isDebugMode = false;

    /// <summary>
    /// Validates references, set sensor camera to not render automatically
    /// and subscribes to 
    /// </summary>
    private void OnEnable()
    {
        _isValid = Validate();
        if (!_isValid)
        {
            return;
        }

        sensorCapture.GetSensorCamera.enabled = false;
        Camera.onPreRender += HandleCameraPreRender;

        InitSettings();
        SubscribeToSensorShaderValuesUpdate();
        SubscribeToOverlaySettings();
        SubscribeToSimulationController();
    }

    /// <summary>
    /// Unsubscribes everything OnEnable subscribed.
    /// </summary>
    private void OnDisable()
    {
        Camera.onPreRender -= HandleCameraPreRender;

        if (_isValid)
        {
            UnsubscribeFromSensorShaderValuesUpdate();
            UnsubscribeFromOverlaySettings();
            UnsubscribeFromSimulationController();
        }
    }

    private void InitSettings()
    {
        sensorCapture.MaxHeatmapDistance = overlaySettings.MaxHeatmapDistance;
        sensorCapture.MaxIRLightingRange = overlaySettings.MaxIRLightingRange;
    }

    /// <summary>
    /// Subscribes to SimulationController Reset
    /// </summary>
    private void SubscribeToSimulationController()
    {
        if (SimulationController.Instance == null)
        {
            return;
        }

        SimulationController.Instance.OnReset.AddListener(ResetObject);
    }

    /// <summary>
    /// Cleanup
    /// </summary>
    private void UnsubscribeFromSimulationController()
    {
        if (SimulationController.Instance == null)
        {
            return;
        }

        SimulationController.Instance.OnReset.RemoveListener(ResetObject);
    }

    /// <summary>
    /// Subscribes to overlay setting Quantity value changes and sets them where needed. 
    /// </summary>
    private void SubscribeToOverlaySettings()
    {
        overlaySettings.SensorResolutionQuantity.onValueChanged.AddListener(value => sensorCapture.SetRenderTextureResulution(sensorCapture.HeatmapTexture, (int)value, (int)value));
        overlaySettings.SensorResolutionQuantity.onValueChanged.AddListener(value => sensorCapture.SetRenderTextureResulution(sensorCapture.HeatmapDepthTexture, (int)value, (int)value));
        overlaySettings.SensorResolutionQuantity.onValueChanged.AddListener(value => sensorCapture.SetRenderTextureResulution(sensorCapture.AbsorptionTexture, (int)value, (int)value));
        
        sensorCapture.SetRenderTextureResulution(sensorCapture.HeatmapTexture, overlaySettings.SensorXResolution, overlaySettings.SensorYResolution);
        sensorCapture.SetRenderTextureResulution(sensorCapture.HeatmapDepthTexture, overlaySettings.SensorXResolution, overlaySettings.SensorYResolution);
        sensorCapture.SetRenderTextureResulution(sensorCapture.AbsorptionTexture, overlaySettings.SensorXResolution, overlaySettings.SensorYResolution);
        overlaySettings.DebugModeQuantity.onValueChanged.AddListener(value => _isDebugMode = value > 0.5f);
        _isDebugMode = overlaySettings.DebugModeQuantity.Value > 0.5f;
    }

    /// <summary>
    /// Cleanup
    /// </summary>
    private void UnsubscribeFromOverlaySettings()
    {
        overlaySettings.SensorResolutionQuantity.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Subscribes to the UpdateSensorShaderValues in a way that if Quantity values change the shader gets updated.
    /// This way the scripts own their Resources and this only links them together.
    /// </summary>
    private void SubscribeToSensorShaderValuesUpdate()
    {
        _onMaxColorDistanceChanged = value => ToFSensorShaderFeed.UpdateScanMaxDistanceShaderValues(value, sensorCapture.HeatmapMaterial);
        _onMaxIRLightingRangeChanged = value => ToFSensorShaderFeed.UpdateScanMaxDistanceShaderValues(value, sensorCapture.AbsorptionMaterial);

        sensorCapture.MaxHeatmapDistanceQuantity.onValueChanged.AddListener(_onMaxColorDistanceChanged);
        sensorCapture.MaxIRLightingRangeQuantity.onValueChanged.AddListener(_onMaxIRLightingRangeChanged);

        ToFSensorShaderFeed.UpdateScanMaxDistanceShaderValues(sensorCapture.MaxHeatmapDistance, sensorCapture.HeatmapMaterial);
        ToFSensorShaderFeed.UpdateScanMaxDistanceShaderValues(sensorCapture.MaxIRLightingRange, sensorCapture.AbsorptionMaterial);
    }

    /// <summary>
    /// For Cleanup
    /// </summary>
    private void UnsubscribeFromSensorShaderValuesUpdate()
    {
        sensorCapture.MaxHeatmapDistanceQuantity.onValueChanged.RemoveListener(_onMaxColorDistanceChanged);
        sensorCapture.MaxIRLightingRangeQuantity.onValueChanged.RemoveListener(_onMaxIRLightingRangeChanged);
    }

    /// <summary>
    /// Reset Button 
    /// TODO reset object location and buttons 
    /// </summary>
    public void ResetObject()
    {
        this.enabled = false;
        this.enabled = true;
    }

    /// <summary>
    /// Turns Demonstration Camera on and off
    /// </summary>
    public void SetDemoActive(bool active)
    {
        if (_demoActive == active)
        {
            return;
        }

        _demoActive = active;
        demoCameraScreen.SwitchScreenOnOff(_demoActive);

        if (!_demoActive)
        {
            _storedColourIntensity = overlaySettings.ColourIntensity;
            overlaySettings.ColourIntensity = 0f;
        }
        else
        {
            overlaySettings.ColourIntensity = _storedColourIntensity;
        }

    }
    /// <summary>
    /// Demonstration mode from depth to absorption
    /// </summary>
    public void SwitchMode(float value)
    {
        switch (captureMode)
        {
            case CaptureMode.DepthInformation:
                if(value < 0.5f)
                {
                    return;
                }
                captureMode = CaptureMode.AbsorptionValues;
                demoCameraScreen.SetRenderTexture(sensorCapture.AbsorptionTexture);
                break;
            case CaptureMode.AbsorptionValues:
                if(value > 0.5f)
                {
                    return;
                }
                captureMode = CaptureMode.DepthInformation;
                demoCameraScreen.SetRenderTexture(sensorCapture.HeatmapTexture);
                break;
        }
    }

    /// <summary>
    /// Pushes overlay uniforms for whichever active player camera is about to render.
    /// Depending if debug is active a different shader is used.
    /// </summary>
    private void HandleCameraPreRender(Camera playerCamera)
    {
        var playerCameras = cameraResolver.GetPlayerCameras();
        for (int i = 0; i < playerCameras.Count; i++)
        {
            if (playerCamera != playerCameras[i].Camera || playerCameras[i].Overlay == null)
            {
                continue;
            }
            if (!_isDebugMode)
            {
                // for blit onrender
                playerCameras[i].Overlay.ActiveMaterial = playerCameras[i].Overlay.ScanVisualizerMaterial;

                // for shader value passing
                PlayerShaderFeed.UpdatePlayerWorldOverlayShaderValues(
                    playerCamera: playerCameras[i].Camera,
                    scanCamera: sensorCapture.GetSensorCamera,
                    material: playerCameras[i].Overlay.ScanVisualizerMaterial,
                    scanColorTex: GetRenderTextureBasedOnMode(),
                    scanDepthTex: sensorCapture.HeatmapDepthTexture,
                    settings: overlaySettings
                );
            }
            else
            {
                // for blit onrender
                playerCameras[i].Overlay.ActiveMaterial = playerCameras[i].Overlay.ScanVisualizerDebugMaterial;

                // for shader value passing
                PlayerShaderFeed.UpdatePlayerWorldOverlayShaderValuesDebugModes(
                    playerCamera: playerCameras[i].Camera,
                    scanCamera: sensorCapture.GetSensorCamera,
                    material: playerCameras[i].Overlay.ScanVisualizerDebugMaterial,
                    scanColorTex: GetRenderTextureBasedOnMode(),
                    scanDepthTex: sensorCapture.HeatmapDepthTexture,
                    settings: overlaySettings
                );
            }
        }
    }
    
    private RenderTexture GetRenderTextureBasedOnMode()
    {
        RenderTexture renderTexture;
            switch (captureMode)
            {
                case CaptureMode.DepthInformation:
                    renderTexture = sensorCapture.HeatmapTexture;
                    break;

                case CaptureMode.AbsorptionValues:
                    renderTexture = sensorCapture.AbsorptionTexture;
                    break;

                default:
                    renderTexture = sensorCapture.HeatmapTexture;
                    break;
            }
        return renderTexture;
    }

    /// <summary>
    /// Forces the sensor camera to capture before any player camera renders
    /// </summary>
    private void LateUpdate()
    {
        if (!_isValid || !_demoActive)
        {
            return;
        }
        sensorCapture.GetSensorCamera.Render();
    }

    /// <summary>
    /// Validates requirements
    /// </summary>
    private bool Validate()
    {
        bool valid = true;

        if (sensorCapture == null)
        {
            Debug.LogError("ToF Demonstration: sensor capture not assigned.", this);
            valid = false;
        }
        if (cameraResolver == null)
        {
            Debug.LogError("ToF Demonstration: camera resolver not assigned.", this);
            valid = false;
        }
        if (overlaySettings == null)
        {
            Debug.LogError("ToF Demonstration: overlay settings not assigned.", this);
            valid = false;
        }

        return valid;
    }
}
